using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any component that wants to perform dashes
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerDashPerformer
    {
        void Request();
        void Stop();
        void Perform();
        UnityEvent<GameObject> DashPerformed { get; }
    }
}
