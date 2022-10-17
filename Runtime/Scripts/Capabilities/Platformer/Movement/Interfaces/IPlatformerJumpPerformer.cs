using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any component that wants to perform jumps 
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerJumpPerformer
    {
        bool performing { get; }
        bool performingExtra { get; }

        void Request();
        void Perform();
        void PerformExtrajump();
        void Stop();
        void Lock(bool shouldLock);

        UnityEvent<bool> JumpUpdate { get; }
        UnityEvent<bool> ExtraJumpUpdate { get; }
    }
}
