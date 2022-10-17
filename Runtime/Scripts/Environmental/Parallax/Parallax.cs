using System.Collections.Generic;
using UnityEngine;
using H2DT.NaughtyAttributes;
using System.Linq;

namespace H2DT.Environmental
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Environmental/Parallax/Parallax")]
    public class Parallax : MonoBehaviour
    {
        #region Inspector

        [Header("Needed Objects")]
        [SerializeField]
        protected Camera _parallaxCamera;

        [SerializeField]
        protected GameObject _subject;

        [Header("Config")]
        [SerializeField]
        [Space]
        protected bool _on = true;

        [Tooltip("Automatically grow layers width 3 times. Use this only with seamless (loopable) sprites")]
        [InfoBox("If Grow Layers is marked sprites used must have their Mesh Type set as Full Rect", EInfoBoxType.Warning)]
        [SerializeField]
        [Space]
        protected bool _growLayers = true;

        [Tooltip("If do not want the effect being applied on Y Axis")]
        [SerializeField]
        [Space]
        protected bool _lockY = false;

        [Tooltip("If do not want the effect being applied on X Axis")]
        [SerializeField]
        protected bool _lockX = false;

        [Tooltip("The speed factor applied to the parallax effect")]
        [InfoBox("The parallax effect speed is calculated based on the layer distance from the subject. Change these in orther to tweak the effect")]
        [SerializeField]
        [Space]
        protected Vector2 _speedFactor = new Vector2(1f, 1f);

        [Tooltip("Mark this if your layer images should be looped causing an infinite background effect")]
        [Label("Infinite Background")]
        [SerializeField]
        [Space]
        protected bool _infinite = false;

        [Tooltip("Mark this if you want the component to auto arrange its layers. You can still override specific layers positions")]
        [Label("Arrange Layers Automatically")]
        [SerializeField]
        [Space]
        protected bool _autoArrangeLayers = false;

        #endregion

        #region Properties

        protected float cameraDistanceFromSubject => _subject.transform.position.z - _parallaxCamera.transform.position.z;
        protected float sightFromSubject => _parallaxCamera.farClipPlane - cameraDistanceFromSubject;

        #endregion

        // Getters
        public bool on => _on;
        public Camera parallaxCamera => _parallaxCamera;
        public GameObject subject => _subject;
        public bool growLayers => _growLayers;
        public bool lockY => _lockY;
        public bool lockX => _lockX;
        public Vector2 speedFactor => _speedFactor;
        public bool infinite => _infinite;

        #region Mono

        protected void Awake()
        {
            SetupLayers();
        }

        #endregion


        protected virtual void SetupLayers()
        {
            List<ParallaxLayer> layers = GetComponentsInChildren<ParallaxLayer>().ToList();
            ArrangeForeground(layers);
            ArrangeBackground(layers);
        }

        protected virtual void ArrangeForeground(List<ParallaxLayer> layers)
        {

            List<ParallaxLayer> foregroundLayers = layers.Where(l => l.layerType == ParallaxLayerType.Foreground).ToList();

            float distanceModifier = 5f;

            for (int i = foregroundLayers.Count - 1; i >= 0; i--)
            {
                ParallaxLayer layer = foregroundLayers[i];
                if (_autoArrangeLayers && !layer.keepPosition)
                {
                    float distance = (1 + i) * distanceModifier;
                    Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, -distance);
                    layer.SetReferencePos(pos);
                }
                else
                {
                    Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, layer.transform.position.z);
                    layer.SetReferencePos(pos);
                }
            }
        }

        protected virtual void ArrangeBackground(List<ParallaxLayer> layers)
        {
            List<ParallaxLayer> backgroundLayers = layers.Where(l => l.layerType == ParallaxLayerType.Background).ToList();

            float distanceModifier = sightFromSubject / backgroundLayers.Count;

            for (int i = 0; i < backgroundLayers.Count; i++)
            {
                ParallaxLayer layer = backgroundLayers[i];
                if (_autoArrangeLayers && !layer.keepPosition)
                {
                    if (i == 0) // first layer
                    {
                        float distance = distanceModifier / 2;
                        Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, distance);
                        layer.SetReferencePos(pos);
                    }
                    else if (i == backgroundLayers.Count - 1) // last layer
                    {
                        float distance = sightFromSubject - 1f;
                        Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, distance);
                        layer.SetReferencePos(pos);
                    }
                    else // middle layers proportianally layed out
                    {
                        float distance = i * distanceModifier;
                        Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, distance);
                        layer.SetReferencePos(pos);
                    }
                }
                else
                {
                    Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, layer.transform.position.z);
                    layer.SetReferencePos(pos);
                }
            }
        }

        public virtual void TurnOn()
        {
            _on = true;
        }

        public virtual void TurnOff()
        {
            _on = false;
        }

        public virtual void Toggle()
        {
            _on = _on ? false : true;
        }
    }

    public enum ParallaxLayerType
    {
        Foreground,
        Background
    }
}
