using UnityEngine;
using H2DT.NaughtyAttributes;

namespace H2DT.Environmental
{

    [AddComponentMenu("Handy 2D Tools/Platformer/Environmental/Parallax/ParallaxLayer")]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxLayer : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        protected Parallax _parallax;

        [SerializeField]
        protected ParallaxLayerType _layerType = ParallaxLayerType.Background;

        [Tooltip("In case you want to override the auto arrange and keep the position of this particular layer")]
        [SerializeField]
        protected bool _keepPosition = false;

        [Header("Speed Factor")]
        [SerializeField]
        protected bool _overrideSpeedFactor = false;

        [SerializeField]
        [ShowIf("overrideSpeedFactor")]
        protected Vector2 _speedFactor = new Vector2(1f, 1f);

        #endregion

        #region Fields

        protected SpriteRenderer _spriteRenderer;
        protected Vector3 _referencePos;
        protected float _textureUnitSizeX;

        #endregion

        #region Properties

        // Overrides
        protected Vector2 speedFactor => _overrideSpeedFactor ? _speedFactor : _parallax.speedFactor;

        // Distances from the subject
        protected float travelX => _parallax.parallaxCamera.transform.position.x - _referencePos.x;
        protected float travelY => _parallax.parallaxCamera.transform.position.y - _referencePos.y;

        // Calculating the parallax factor
        protected float distanceFromSubject => _referencePos.z - _parallax.subject.transform.position.z;
        protected float clipPlane => _parallax.parallaxCamera.transform.position.z + (distanceFromSubject > 0 ? _parallax.parallaxCamera.farClipPlane : -_parallax.parallaxCamera.nearClipPlane);
        protected float parallaxFactor => (Mathf.Abs(distanceFromSubject) / clipPlane);

        // Calculating new positions
        protected float xCalc => !_parallax.lockX ? _referencePos.x + (travelX * parallaxFactor * speedFactor.x) : _referencePos.x;
        protected float yCalc => !_parallax.lockY ? _referencePos.y + (travelY * parallaxFactor * speedFactor.y) : _referencePos.y;

        // What value should be used as an update
        protected float newX => _parallax.on ? xCalc : transform.position.x;
        protected float newY => _parallax.on ? yCalc : transform.position.y;

        #endregion

        #region Getters

        public ParallaxLayerType layerType => _layerType;
        public bool keepPosition => _keepPosition;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (_parallax == null)
                _parallax = GetComponentInParent<Parallax>();

            if (_parallax == null)
                Debug.LogError($"{gameObject.name} is not attached to a Parallax object!");

            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_parallax != null && _parallax.growLayers)
            {
                _spriteRenderer.drawMode = SpriteDrawMode.Tiled;
                _spriteRenderer.size = new Vector2(_spriteRenderer.bounds.size.x * 3, _spriteRenderer.size.y);
            }

            _textureUnitSizeX = _spriteRenderer.sprite.texture.width / _spriteRenderer.sprite.pixelsPerUnit;
        }

        protected virtual void FixedUpdate()
        {
            Apply();
        }

        #endregion

        protected virtual void Apply()
        {
            transform.position = new Vector3(newX, newY, _referencePos.z);

            EvaluateAndSnapToCamera();
        }

        protected virtual void EvaluateAndSnapToCamera()
        {
            if (!_parallax.infinite) return;

            float cameraDistance = _parallax.parallaxCamera.transform.position.x - transform.position.x;

            if (Mathf.Abs(cameraDistance) >= _textureUnitSizeX)
            {
                float offsetPositionX = cameraDistance % _textureUnitSizeX;
                _referencePos.x = _parallax.parallaxCamera.transform.position.x - offsetPositionX;
                if (_layerType == ParallaxLayerType.Background)
                {
                    transform.position = new Vector3(_parallax.parallaxCamera.transform.position.x + offsetPositionX, transform.position.y, _referencePos.z);
                }
                else if (_layerType == ParallaxLayerType.Foreground)
                {
                    transform.position = new Vector3(_parallax.parallaxCamera.transform.position.x - offsetPositionX, transform.position.y, _referencePos.z);
                }
            }
        }

        public void SetReferencePos(Vector3 pos)
        {
            _referencePos = pos;
        }

    }
}
