using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to request slide starting 
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerSlideHandler
    {
        /// <summary>
        /// Slide performers should listen to this in order to know when request is made
        /// </summary>
        /// <value> The direction sign </value>
        UnityEvent SlideRequest { get; }
    }
}
