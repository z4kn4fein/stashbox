using Ronin.Common;
using Stashbox.Attributes;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.ContainerExtensions.MethodInjection
{
    public class MethodInjectionExtension : IRegistrationExtension, IPostBuildExtension
    {
        private readonly ConcurrentKeyValueStore<Type, MethodInfoCache> methodInfoRepository;

        public MethodInjectionExtension()
        {
            this.methodInfoRepository = new ConcurrentKeyValueStore<Type, MethodInfoCache>();
        }

        public void OnRegistration(IBuilderContext builderContext, RegistrationInfo registrationInfo)
        {
            var methods = registrationInfo.TypeTo.GetTypeInfo().DeclaredMethods
               .Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null &&
                      methodInfo.GetParameters() != null &&
                      methodInfo.GetParameters().All(parameter =>
                          builderContext.ResolverSelector.CanResolve(builderContext, new TypeInformation
                          {
                              DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                              Type = parameter.ParameterType
                          })))
               .Select(methodInfo =>
               {
                   var parameters = methodInfo.GetParameters().Select(parameter =>
                   {
                       var parameterInfo = new ParameterInformation
                       {
                           DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                           Type = parameter.ParameterType,
                           ParameterInfo = parameter
                       };

                       Resolver resolver;
                       builderContext.ResolverSelector.TryChooseResolver(builderContext, parameterInfo, out resolver);
                       return new ResolutionParameter
                       {
                           ParameterInfo = parameterInfo,
                           Resolver = resolver
                       };
                   });

                   return new MethodInfoItem
                   {
                       Parameters = new HashSet<ResolutionParameter>(parameters),
                       MethodDelegate = ExpressionBuilder.BuildMethodExpression(methodInfo,
                                        parameters.Select(resolutionParameter => resolutionParameter.ParameterInfo), registrationInfo.TypeTo)
                   };
               });

            this.methodInfoRepository.Add(registrationInfo.TypeTo, new MethodInfoCache
            {
                Methods = new HashSet<MethodInfoItem>(methods)
            });
        }

        public object PostBuild(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            MethodInfoCache methodCache;
            if (this.methodInfoRepository.TryGet(instance.GetType(), out methodCache))
            {
                var methods = methodCache.Methods.ToDictionary(key => key, method =>
                              method.Parameters.Select(parameter =>
                              {
                                  if (resolutionInfo.OverrideManager.ContainsValue(parameter.ParameterInfo))
                                  {
                                      return resolutionInfo.OverrideManager.GetOverriddenValue(parameter.ParameterInfo.Type, parameter.ParameterInfo.DependencyName);
                                  }
                                  else if (parameter.ParameterValue != null)
                                  {
                                      return parameter.ParameterValue;
                                  }
                                  else
                                      return parameter.Resolver.Resolve(new ResolutionInfo
                                      {
                                          FactoryParams = resolutionInfo.FactoryParams,
                                          OverrideManager = resolutionInfo.OverrideManager,
                                          ResolveType = new TypeInformation
                                          {
                                              DependencyName = parameter.ParameterInfo.DependencyName,
                                              Type = parameter.ParameterInfo.Type
                                          }
                                      });
                              }));

                foreach (var method in methods)
                {
                    method.Key.MethodDelegate(instance, method.Value.ToArray());
                }
            }

            return instance;
        }
    }
}
