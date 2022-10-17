using UnityEngine;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to check if it is grounded or not
    /// should implement this Interface.
    /// </summary>
    public interface IPlatformerGroundingChecker
    {
        LayerMask whatIsGround { get; set; }
    }
}
