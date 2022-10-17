using System.Collections;
using H2DT.Debugging;
using H2DT.Environmental.Platforming;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Platforming/PlatformFall")]
    public class PlatformFall : DocumentedComponent, IPlatformFallPerformer
    {

        #region Inspector

        [Header("Dependencies")]
        [InfoBox("If you prefer you can read the docs on how to feed this component directly through one of your scripts.")]
        [Label("Seek Handler")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformFallHandler you can mark this and it will subscribe to its events.")]
        [SerializeField]
        [Space]
        protected bool _seekPlatformFallHandler = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerGroundingProvider you can mark this and it will subscribe to its events. RaycastGroundingChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool _seekGroundingProvider = false;

        [Header("Layers")]
        [Tooltip("Inform what layers should be considered a possibility to descend")]
        [SerializeField]
        [Space]
        protected LayerMask _whatIsPlatform;

        [Header("Collider")]
        [Tooltip("Inform what collider from the subject should be used as reference")]
        [SerializeField]
        [Space]
        protected Collider2D _descenderCollider;

        [Header("Events")]
        [SerializeField]
        [Space]
        protected UnityEvent<bool> _descending;

        #endregion

        #region  Components

        protected IPlatformFallHandler _platformFallHandler;
        protected IPlaftormerGroundingProvider _groundingProvider;

        #endregion

        #region Fields

        protected bool _grounded = false;

        #endregion        

        #region Properties

        public LayerMask whatIsPlatform { get { return _whatIsPlatform; } set { _whatIsPlatform = value; } }
        public Collider2D PlatformFallPerformerCollider { get { return _descenderCollider; } set { _descenderCollider = value; } }

        #endregion

        #region Getters

        public UnityEvent<bool> Falling => _descending;

        #endregion

        #region Mono

        protected void Awake()
        {
            FindComponents();
        }

        protected void OnEnable()
        {
            SubscribeSeekers();
        }

        protected void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion


        #region Logic

        public virtual void Request()
        {
            if (!_grounded) return;


            Vector2 rightCastPosition = (Vector2)_descenderCollider.bounds.center + new Vector2(_descenderCollider.bounds.extents.x, -_descenderCollider.bounds.extents.y);
            Vector2 leftCastPosition = (Vector2)_descenderCollider.bounds.center + new Vector2(-_descenderCollider.bounds.extents.x, -_descenderCollider.bounds.extents.y);

            RaycastHit2D rightHit = Physics2D.Raycast(rightCastPosition, Vector2.down, 1f, _whatIsPlatform);
            RaycastHit2D leftHit = Physics2D.Raycast(leftCastPosition, Vector2.down, 1f, _whatIsPlatform);

            if (rightHit) { EvaluatePlatformAndFall(rightHit.collider.gameObject); return; }
            if (leftHit) { EvaluatePlatformAndFall(leftHit.collider.gameObject); return; }
        }

        protected void EvaluatePlatformAndFall(GameObject colliderObj)
        {
            IFallablePlatform fallablePlatform = colliderObj.GetComponent<IFallablePlatform>();
            if (fallablePlatform != null)
                StartCoroutine(DisableOneWayPlatformCollider(fallablePlatform));
        }

        protected IEnumerator DisableOneWayPlatformCollider(IFallablePlatform fallablePlatform)
        {
            Collider2D collider = fallablePlatform.PlatformCollider;

            if (collider == null)
            {
                Log.Warning($"{fallablePlatform.gameObject.name} has no Collider2D component");
                yield break;
            }

            PlatformEffector2D platformEffector2D = fallablePlatform.PlatformEffector;

            if (platformEffector2D != null)
            {
                platformEffector2D.rotationalOffset = 180f;
            }

            collider.enabled = false;
            _descending.Invoke(true);

            yield return new WaitForSeconds(fallablePlatform.DisableDuration);

            if (platformEffector2D != null)
            {
                platformEffector2D.rotationalOffset = 0;
            }

            collider.enabled = true;
            _descending.Invoke(false);
        }

        #endregion

        #region Feeding

        public virtual void UpdateGrounding(bool newGrounding)
        {
            _grounded = newGrounding;
        }

        #endregion

        #region Handling

        public virtual void OnFallRequest()
        {
            Request();
        }

        #endregion

        #region Components and events

        protected virtual void FindComponents()
        {
            SeekComponent<IPlatformFallHandler>(_seekPlatformFallHandler, ref _platformFallHandler);
            SeekComponent<IPlaftormerGroundingProvider>(_seekGroundingProvider, ref _groundingProvider);
        }

        protected virtual void SubscribeSeekers()
        {
            _platformFallHandler?.PlatformFallRequest.AddListener(OnFallRequest);
            _groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
        }

        protected virtual void UnsubscribeSeekers()
        {
            _platformFallHandler?.PlatformFallRequest.RemoveListener(OnFallRequest);
            _groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
        }

        #endregion


        #region Handy Component

        protected override string docPath => "core/character-controller/platformer/abilities/platforming/platform-fall.html";

        #endregion
    }
}
