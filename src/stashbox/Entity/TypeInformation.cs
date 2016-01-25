using System;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents type information about a dependency.
    /// </summary>
    public class TypeInformation
    {
        /// <summary>
        /// The reflected type of the dependency.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The reflected type of the parent of the dependency.
        /// </summary>
        public Type ParentType { get; set; }

        /// <summary>
        /// The name of the dependency.
        /// </summary>
        public string DependencyName { get; set; }

        /// <summary>
        /// Custom attributes of the dependency.
        /// </summary>
        public Attribute[] CustomAttributes { get; set; }

        /// <summary>
        /// The member name of the dependency.
        /// </summary>
        public string MemberName { get; set; }
    }
}