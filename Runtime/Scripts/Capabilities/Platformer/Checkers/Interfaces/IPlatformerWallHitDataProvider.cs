using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to give information about being or not hitting walls
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerWallHitDataProvider
    {
        UnityEvent<WallHitData> WallHitDataUpdate { get; }
    }
}
