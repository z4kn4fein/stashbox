using System.Diagnostics;
using System.Xml.Linq;

namespace Stashbox
{
    /// <summary>
    /// Describes a wrapper for services with additional metadata.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TMeta">The additional metadata type.</typeparam>
    public sealed class Metadata<TService, TMeta>
    {
        /// <summary>
        /// The service.
        /// </summary>
        public readonly TService Service;

        /// <summary>
        /// The additional metadata.
        /// </summary>
        public readonly TMeta Data;

        /// <summary>
        /// Constructs a <see cref="Metadata{TService, TMeta}"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="data">The additional metadata.</param>
        public Metadata(TService service, TMeta data)
        {
            this.Service = service;
            this.Data = data;
        }
    }
}
