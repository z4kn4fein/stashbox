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

        public void OnRegistration(IBuilderContext builderContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            var methods = registrationInfo.TypeTo.GetTypeInfo().DeclaredMethods
               .Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null &&
                                    methodInfo.GetParameters() != null &&
                                    methodInfo.GetParameters().All(parameter =>
                                        builderContext.ResolverSelector.CanResolve(builderContext, new TypeInformation
                                        {
                                            DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                                            Type = parameter.ParameterType
                                        }) || (injectionParameters != null && injectionParameters.Any(param => param.Name == parameter.Name))))
               .Select(methodInfo =>
               {
                   var parameters = methodInfo.GetParameters().Select(parameter =>
                   {
                       var resolutionTarget = new ResolutionTarget
                       {
                           TypeInformation = new TypeInformation
                           {
                               DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                               Type = parameter.ParameterType
                           },
                           ResolutionTargetValue = injectionParameters?.FirstOrDefault(param => param.Name == parameter.Name)?.Value,
                           ResolutionTargetName = parameter.Name
                       };

                       Resolver resolver;
                       builderContext.ResolverSelector.TryChooseResolver(builderContext, resolutionTarget.TypeInformation, out resolver);
                       resolutionTarget.Resolver = resolver;
                       return resolutionTarget;
                   });

                   var resolutionParameters = parameters as ResolutionTarget[] ?? parameters.ToArray();
                   return new MethodInfoItem
                   {
                       Parameters = new HashSet<ResolutionTarget>(resolutionParameters),
                       MethodDelegate = ExpressionBuilder.BuildMethodExpression(methodInfo,
                                        resolutionParameters.Select(resolutionParameter => resolutionParameter.TypeInformation), registrationInfo.TypeTo)
                   };
               });

            this.methodInfoRepository.Add(registrationInfo.TypeTo, new MethodInfoCache
            {
                Methods = new HashSet<MethodInfoItem>(methods)
            });
        }

        public object PostBuild(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            MethodInfoCache methodCache;
            if (this.methodInfoRepository.TryGet(instance.GetType(), out methodCache))
            {
                var methods = methodCache.Methods.ToDictionary(key => key, method =>
                              method.Parameters.Select(parameter =>
                              {
                                  if (resolutionInfo.OverrideManager.ContainsValue(parameter.TypeInformation))
                                  {
                                      return resolutionInfo.OverrideManager.GetOverriddenValue(parameter.TypeInformation.Type, parameter.TypeInformation.DependencyName);
                                  }
                                  else if (parameter.ResolutionTargetValue != null)
                                  {
                                      return parameter.ResolutionTargetValue;
                                  }
                                  else
                                      return parameter.Resolver.Resolve(new ResolutionInfo
                                      {
                                          FactoryParams = resolutionInfo.FactoryParams,
                                          OverrideManager = resolutionInfo.OverrideManager,
                                          ResolveType = new TypeInformation
                                          {
                                              DependencyName = parameter.TypeInformation.DependencyName,
                                              Type = parameter.TypeInformation.Type
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
