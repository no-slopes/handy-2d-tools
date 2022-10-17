using UnityEngine;
using H2DT.NaughtyAttributes;

namespace H2DT.Environmental
{

    [AddComponentMenu("Handy 2D Tools/Platformer/Environmental/Parallax/ParallaxTrigger")]
    public class ParallaxTrigger : MonoBehaviour
    {
        [SerializeField]
        protected Parallax _parallax;

        [Tag]
        [SerializeField]
        [InfoBox("Without this the trigger will not work", EInfoBoxType.Warning)]
        [Label("Tag")]
        protected string _tagField;

        protected virtual void Awake()
        {
            if (_parallax == null)
                _parallax = GetComponentInParent<Parallax>();

            if (_parallax == null)
                Debug.LogError($"{gameObject.name} is not attached to a Parallax object!");
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == _tagField)
            {
                _parallax.Toggle();
            }
        }
    }
}
