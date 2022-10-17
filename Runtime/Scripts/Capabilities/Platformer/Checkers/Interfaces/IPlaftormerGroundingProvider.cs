using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to give information about being or not grounded
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlaftormerGroundingProvider
    {
        bool grounded { get; }
        UnityEvent<bool> GroundingUpdate { get; }
    }
}
