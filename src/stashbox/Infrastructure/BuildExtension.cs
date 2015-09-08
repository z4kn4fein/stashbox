
using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    public abstract class BuildExtension
    {
        public virtual void OnRegistration(IBuilderContext builderContext, RegistrationInfo registrationInfo)
        { }

        public virtual void PreBuild(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        { }

        public virtual object PostBuild(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            throw new Exception("Base PostBuild method was invoked");
        }
    }
}
