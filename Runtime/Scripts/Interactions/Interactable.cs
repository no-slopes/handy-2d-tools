using UnityEngine;

namespace H2DT.Interactions
{
    public class Interactable : MonoBehaviour
    {
        #region Editor

        [Header("Interactable Objects")]
        [SerializeField]
        protected GameObject interacterObj;

        [Header("Interactable Config")]
        [SerializeField][Range(0.1f, 5f)] protected float actionDistanceRange = 1f;
        [SerializeField][Range(0.1f, 20f)] protected float affordanceDistanceRange = 1f;
        [Header("Interactable Affordance")]
        [SerializeField] protected GameObject affordanceObj;
        [SerializeField][Range(0.1f, 10f)] protected float affordanceYoffset = 1;

        #endregion

        #region Properties
        protected IInteracter interacter;
        protected bool currentDismissing;

        protected bool interacterInActionRange = false;
        protected bool interacterInAffordanceRange = false;
        protected bool usedAlready = false;

        #endregion

        #region Getters

        /// <summary>
        /// The Raw distance between interacter and interactable considering direction 
        /// (positive or negative)
        /// </summary>
        protected float RawDistance => Vector2.Distance(interacter.gameObject.transform.position, transform.position);

        /// <summary>
        /// The distance between interacter and interactable disregarding direction.
        /// It is the Absolute value of RawDistance
        /// </summary>
        protected float Distance => Mathf.Abs(RawDistance);

        /// <summary>
        /// True if interacter is in action range
        /// </summary>
        protected bool InteracterInActionRange => Distance <= actionDistanceRange && interacter.CanInteract;

        /// <summary>
        /// True if interacter is in affordance range
        /// </summary>
        protected bool InteracterInAffordanceRange => Distance <= affordanceDistanceRange && interacter.CanInteract;

        /// <summary>
        /// True if interacter is in interaction range
        /// </summary>
        protected bool InteracterCanInteract => InteractionEvaluation();

        #endregion

        #region Mono
        protected virtual void Awake()
        {
            interacter = interacterObj.GetComponent<IInteracter>();
        }

        protected virtual void Update()
        {

        }

        private void OnDisable()
        {

        }

        #endregion

        #region Logic

        protected virtual bool InteractionEvaluation()
        {
            if (interacter == null || !interacter.CanInteract) return false;
            if (!InteracterInActionRange) return false; // Case Subject not in range
            return false;
        }

        #endregion
    }
}
