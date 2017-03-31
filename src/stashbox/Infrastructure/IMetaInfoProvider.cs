using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a meta info provider.
    /// </summary>
    public interface IMetaInfoProvider
    {
        /// <summary>
        /// True if the containing type has generic type constraints.
        /// </summary>
        bool HasGenericTypeConstraints { get; }

        /// <summary>
        /// Tries to get the best constructor for resolution.
        /// </summary>
        /// <param name="constructor">The selected constructor.</param>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns>True if the selection was successful, otherwise false.</returns>
        bool TryChooseConstructor(out ResolutionConstructor constructor, ResolutionInfo resolutionInfo);

        /// <summary>
        /// Gets the resolution method of the registered service.
        /// </summary>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns>The <see cref="ResolutionMethod"/> collection.</returns>
        ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo);

        /// <summary>
        /// Gets the resolution members of the registered service.
        /// </summary>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns>The <see cref="ResolutionMember"/> collection.</returns>
        ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo);

        /// <summary>
        /// Validates a type against the generic constraints of the registered service.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns>True if the validation was successful, otherwise false.</returns>
        bool ValidateGenericContraints(Type type);
    }
}
