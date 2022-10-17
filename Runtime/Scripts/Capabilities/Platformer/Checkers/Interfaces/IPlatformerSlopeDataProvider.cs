using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to give information about being or not on a Slope
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerSlopeDataProvider
    {
        UnityEvent<PlatformerSlopeData> SlopeDataUpdate { get; }
    }
}
