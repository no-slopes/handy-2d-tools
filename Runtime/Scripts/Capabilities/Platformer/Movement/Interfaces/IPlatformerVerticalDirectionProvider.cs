
using UnityEngine.Events;
using H2DT.Enums;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to controll vertical directions can use an
    /// event through implementing this Interface.
    /// </summary>
    public interface IPlatformerVerticalDirectionProvider
    {
        /// <summary>
        /// An event wich should be fired to update direction
        /// </summary>
        /// <value> A VerticalDirections representing the direction. </value>
        UnityEvent<VerticalDirection> VerticalDirectionUpdate { get; }

        /// <summary>
        /// An event wich should be fired to update facing direction sign
        /// </summary>
        /// <value> A float representing the the direction. -1 for down and 1 for up. </value>
        UnityEvent<float> VerticalDirectionSignUpdate { get; }
    }
}
