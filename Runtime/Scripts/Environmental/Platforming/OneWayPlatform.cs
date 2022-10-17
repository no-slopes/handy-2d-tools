using H2DT.Capabilities.Platforming;
using H2DT.Debugging;
using UnityEngine;

namespace H2DT.Environmental.Platforming
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PlatformEffector2D))]
    [AddComponentMenu("Handy 2D Tools/Platformer/Environmental/Platforms/OneWayPlatform")]
    public class OneWayPlatform : MonoBehaviour, IFallablePlatform
    {
        [SerializeField]
        protected GameObject subject;

        [SerializeField]
        protected float disableDuration = 0.5f;

        protected IPlatformFallPerformer fallPerformer;

        protected Collider2D platformCollider;
        protected PlatformEffector2D platformEffector;
        protected Collider2D fallPerformerCollider;

        #region Getters

        public Collider2D PlatformCollider => platformCollider;
        public PlatformEffector2D PlatformEffector => platformEffector;
        public float DisableDuration => disableDuration;

        #endregion

        protected void Awake()
        {
            platformCollider = GetComponent<Collider2D>();
            platformEffector = GetComponent<PlatformEffector2D>();

            fallPerformer = subject.GetComponent<IPlatformFallPerformer>();
            fallPerformerCollider = fallPerformer?.PlatformFallPerformerCollider;

            if (platformCollider == null)
            {
                Log.Danger($"{GetType().Name}: Subject has no collider.");
            }
        }

        protected void LateUpdate()
        {
            if (fallPerformerCollider == null) return;

            Vector2 subjectColliderPosition = (Vector2)fallPerformerCollider.bounds.center + new Vector2(0, -fallPerformerCollider.bounds.extents.y);
            Vector2 platformColliderPosition = (Vector2)platformCollider.bounds.center + new Vector2(0, platformCollider.bounds.extents.y);

            if (platformColliderPosition.y < subjectColliderPosition.y)
            {
                platformCollider.enabled = true;
            }
            else
            {
                platformCollider.enabled = false;
            }
        }
    }
}
