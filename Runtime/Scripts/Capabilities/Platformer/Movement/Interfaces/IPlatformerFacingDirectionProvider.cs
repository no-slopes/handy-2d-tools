using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using H2DT.Enums;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to controll facing direction can use an
    /// event through implementing this Interface.
    /// </summary>
    public interface IPlatformerFacingDirectionProvider
    {
        /// <summary>
        /// An event wich should be fired to update facing direction
        /// </summary>
        /// <value> A HorizontalDirections representing the direction. </value>
        UnityEvent<HorizontalDirection> FacingDirectionUpdate { get; }

        /// <summary>
        /// An event wich should be fired to update facing direction sign
        /// </summary>
        /// <value> A float representing the the facing direction. -1 for left and 1 for right. </value>
        UnityEvent<float> FacingDirectionSignUpdate { get; }
    }
}
