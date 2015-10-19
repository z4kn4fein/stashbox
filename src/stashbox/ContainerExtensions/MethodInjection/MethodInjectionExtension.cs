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

        public void OnRegistration(IContainerContext containerContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            var methods = registrationInfo.TypeTo.GetTypeInfo().DeclaredMethods
               .Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null &&
                                    methodInfo.GetParameters() != null &&
                                    methodInfo.GetParameters().All(parameter =>
                                        containerContext.ResolutionStrategy.CanResolve(containerContext, new TypeInformation
                                        {
                                            DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                                            Type = parameter.ParameterType,
                                            ParentType = registrationInfo.TypeTo,
                                            CustomAttributes = new HashSet<Attribute>(parameter.GetCustomAttributes())
                                        }, injectionParameters, parameter.Name)))
               .Select(methodInfo =>
               {
                   var parameters = methodInfo.GetParameters().Select(parameter => containerContext.ResolutionStrategy.BuildResolutionTarget(containerContext, new TypeInformation
                   {
                       DependencyName = parameter.GetCustomAttribute<DependencyAttribute>()?.Name,
                       Type = parameter.ParameterType,
                       ParentType = registrationInfo.TypeTo,
                       CustomAttributes = new HashSet<Attribute>(parameter.GetCustomAttributes())
                   }, injectionParameters, parameter.Name));

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

        public object PostBuild(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            MethodInfoCache methodCache;
            if (!this.methodInfoRepository.TryGet(targetType, out methodCache)) return instance;
            var methods = methodCache.Methods.ToDictionary(key => key, method =>
                method.Parameters.Select(parameter =>
                    containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, parameter, resolutionInfo)));

            foreach (var method in methods)
            {
                method.Key.MethodDelegate(instance, method.Value.ToArray());
            }

            return instance;
        }
    }
}
