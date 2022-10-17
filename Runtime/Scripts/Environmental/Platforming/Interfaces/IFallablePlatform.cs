using UnityEngine;

namespace H2DT.Environmental.Platforming
{
    public interface IFallablePlatform
    {
        GameObject gameObject { get; }

        Collider2D PlatformCollider { get; }
        PlatformEffector2D PlatformEffector { get; }

        float DisableDuration { get; }
    }
}