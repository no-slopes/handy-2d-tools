using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    public interface IPlatformFallPerformer
    {
        Collider2D PlatformFallPerformerCollider { get; set; }
        LayerMask whatIsPlatform { get; set; }

        UnityEvent<bool> Falling { get; }

        void Request();
    }
}