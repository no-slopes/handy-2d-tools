using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any component that wants to handle dashs
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerDashHandler
    {
        /// <summary>
        /// Dash performers should listen to this in order to know when request is made
        /// </summary>
        /// <value></value>
        UnityEvent DashRequest { get; }
    }
}
