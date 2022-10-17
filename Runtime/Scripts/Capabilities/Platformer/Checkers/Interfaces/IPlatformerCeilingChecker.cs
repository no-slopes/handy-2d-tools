using UnityEngine;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to check if it is under ceilings or not
    /// </summary>
    public interface IPlatformerCeilingChecker
    {
        LayerMask whatIsCeiling { get; set; }
    }
}
