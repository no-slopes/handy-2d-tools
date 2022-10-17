
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to give information about slide starting or being stoped
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerSlidePerformer
    {
        bool performing { get; }

        void Request();
        void Stop();
        void Perform();
        void Lock(bool shouldLock);

        UnityEvent<bool> SlideUpdate { get; }
    }
}
