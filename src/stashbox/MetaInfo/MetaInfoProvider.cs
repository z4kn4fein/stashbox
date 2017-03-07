using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoProvider : IMetaInfoProvider
    {
        private readonly IContainerContext containerContext;

        private readonly bool hasInjectionMethod;
        private readonly bool hasInjectionMembers;

        private readonly bool autoMemberInjectionEnabled;
        private readonly Rules.AutoMemberInjection autoMemberInjectionRule;

        private readonly IDictionary<int, Type[]> genericTypeConstraints;
        private ConstructorInformation[] constructors;
        private MethodInformation[] injectionMethods;
        private readonly MemberInformation[] injectionMembers;

        public Type TypeTo { get; private set; }

        public bool HasGenericTypeConstraints { get; }

        public MetaInfoProvider(IContainerContext containerContext, RegistrationContextData registrationData, Type typeTo)
        {
            this.TypeTo = typeTo;
            this.genericTypeConstraints = new Dictionary<int, Type[]>();

            this.autoMemberInjectionEnabled = containerContext.ContainerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled || registrationData.AutoMemberInjectionEnabled;
            this.autoMemberInjectionRule = registrationData.AutoMemberInjectionEnabled ? registrationData.AutoMemberInjectionRule :
                containerContext.ContainerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationRule;

            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.injectionMembers = this.FillMembers(typeInfo).ToArray();
            this.CollectGenericConstraints(typeInfo);
            this.containerContext = containerContext;

            this.hasInjectionMethod = this.injectionMethods.Any();
            this.hasInjectionMembers = this.injectionMembers.Any();

            this.HasGenericTypeConstraints = this.genericTypeConstraints.Count > 0;
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.TryGetConstructor(out resolutionConstructor, resolutionInfo, injectionParameters);

        public ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasInjectionMethod) return null;

            return this.injectionMethods
               .Select(methodInfo => new ResolutionMethod
               {
                   Method = methodInfo.Method,
                   Parameters = methodInfo.Parameters.Select(parameter =>
                   this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
               }).ToArray();
        }

        public ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasInjectionMembers) return null;

            return this.injectionMembers
                .Select(memberInfo => new ResolutionMember
                {
                    Expression = this.containerContext.ResolutionStrategy
                        .BuildResolutionExpression(this.containerContext, resolutionInfo, memberInfo.TypeInformation, injectionParameters),
                    MemberInfo = memberInfo.MemberInfo
                }).Where(info => info.Expression != null).ToArray();
        }

        public bool ValidateGenericContraints(TypeInformation typeInformation)
        {
            var typeInfo = typeInformation.Type.GetTypeInfo();
            var length = typeInfo.GenericTypeArguments.Length;

            for (var i = 0; i < length; i++)
                if (this.genericTypeConstraints.ContainsKey(i) && !this.genericTypeConstraints[i].Contains(typeInfo.GenericTypeArguments[i]))
                    return false;

            return true;
        }

        private bool TryGetConstructor(out ResolutionConstructor resolutionConstructor,
            ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            var usableConstructors = this.CreateResolutionConstructors(resolutionInfo, injectionParameters)
                .Where(ctor => ctor.Parameters.All(param => param != null)).ToArray();

            if (usableConstructors.Any())
            {
                resolutionConstructor = this.SelectBestConstructor(usableConstructors);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }

        private IEnumerable<ResolutionConstructor> CreateResolutionConstructors(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.constructors.Select(constructorInformation => new ResolutionConstructor
            {
                Constructor = constructorInformation.Constructor,
                Parameters = constructorInformation.Parameters.Select(parameter =>
                    this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
            });

        private ResolutionConstructor SelectBestConstructor(IEnumerable<ResolutionConstructor> constructors) =>
            this.containerContext.ContainerConfigurator.ContainerConfiguration.ConstructorSelectionRule(constructors);

        private void CollectGenericConstraints(TypeInfo typeInfo)
        {
            if (!typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
                return;

            foreach (var typeInfoGenericTypeParameter in typeInfo.GenericTypeParameters)
            {
                var paramTypeInfo = typeInfoGenericTypeParameter.GetTypeInfo();
                var pos = paramTypeInfo.GenericParameterPosition;
                var cons = paramTypeInfo.GetGenericParameterConstraints();

                if (cons.Length > 0)
                    this.genericTypeConstraints.Add(pos, cons);
            }
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            this.constructors = infos.Where(info => !info.IsStatic).Select(info => new ConstructorInformation
            {
                Constructor = info,
                Parameters = this.FillParameters(info.GetParameters()).ToArray()
            }).ToArray();
        }

        private void AddMethods(IEnumerable<MethodInfo> infos)
        {
            this.injectionMethods = infos.Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null).Select(info => new MethodInformation
            {
                Method = info,
                Parameters = this.FillParameters(info.GetParameters()).ToArray()
            }).ToArray();
        }

        private IEnumerable<TypeInformation> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameterInfo => new TypeInformation
            {
                Type = parameterInfo.ParameterType,
                DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                ParentType = this.TypeTo,
                CustomAttributes = parameterInfo.GetCustomAttributes().ToArray(),
                ParameterName = parameterInfo.Name,
                HasDefaultValue = parameterInfo.HasDefaultValue(),
                DefaultValue = parameterInfo.DefaultValue
            });
        }

        private IEnumerable<MemberInformation> FillMembers(TypeInfo typeInfo)
        {
            return this.SelectProperties(typeInfo.DeclaredProperties.Where(property => property.CanWrite && !property.IsIndexer()))
                   .Select(propertyInfo => new MemberInformation
                   {
                       TypeInformation = new TypeInformation
                       {
                           Type = propertyInfo.PropertyType,
                           DependencyName = propertyInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                           ParentType = this.TypeTo,
                           CustomAttributes = propertyInfo.GetCustomAttributes().ToArray(),
                           ParameterName = propertyInfo.Name,
                           IsMember = true
                       },
                       MemberInfo = propertyInfo
                   })
                   .Concat(this.SelectFields(typeInfo.DeclaredFields.Where(field => !field.IsInitOnly && !field.IsBackingField()))
                           .Select(fieldInfo => new MemberInformation
                           {
                               TypeInformation = new TypeInformation
                               {
                                   Type = fieldInfo.FieldType,
                                   DependencyName = fieldInfo.GetCustomAttribute<DependencyAttribute>()?.Name,
                                   ParentType = this.TypeTo,
                                   CustomAttributes = fieldInfo.GetCustomAttributes().ToArray(),
                                   ParameterName = fieldInfo.Name,
                                   IsMember = true
                               },
                               MemberInfo = fieldInfo
                           }));
        }

        private IEnumerable<FieldInfo> SelectFields(IEnumerable<FieldInfo> fields)
        {
            if (this.autoMemberInjectionEnabled)
                return fields.Where(fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null ||
                    this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PrivateFields));
            else
                return fields.Where(fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null);
        }

        private IEnumerable<PropertyInfo> SelectProperties(IEnumerable<PropertyInfo> properties)
        {
            if (this.autoMemberInjectionEnabled)
                return properties.Where(property => property.GetCustomAttribute<DependencyAttribute>() != null ||
                    (this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithPublicSetter) && property.HasSetMethod()) ||
                     this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithLimitedAccess));
            else
                return properties.Where(property => property.GetCustomAttribute<DependencyAttribute>() != null);
        }
    }
}
