using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    public interface IPlatformFallHandler
    {
        /// <summary>
        /// Platform fall performers should listen to this in order to know when request is made
        /// </summary>
        /// <value></value>
        UnityEvent PlatformFallRequest { get; }
    }
}
