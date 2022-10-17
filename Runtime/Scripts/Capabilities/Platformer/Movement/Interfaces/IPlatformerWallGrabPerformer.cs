
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any component that wants to perform wall slides 
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerWallGrabPerformer
    {
        bool performing { get; }

        void Request(float movementDirectionSign);
        void Perform(float verticalDirectionSign);
        void Stop();

        UnityEvent<bool> WallGrabUpdate { get; }
    }
}
