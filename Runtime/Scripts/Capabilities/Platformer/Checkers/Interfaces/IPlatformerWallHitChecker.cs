using UnityEngine;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to check if it is hitting walls or not
    /// </summary>
    public interface IPlatformerWallHitChecker
    {
        LayerMask whatIsWall { get; set; }
    }
}
