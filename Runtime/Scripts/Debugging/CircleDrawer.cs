
using UnityEngine;

namespace H2DT.Debugging
{
    [RequireComponent(typeof(LineRenderer))]
    public class CircleDrawer : HandyComponent
    {
        #region Inspector

        [SerializeField]
        private int _steps = 50;

        #endregion

        #region Fields

        private LineRenderer _lineRenderer;

        #endregion

        #region  Mono

        private void Awake()
        {
            FindComponent<LineRenderer>(ref _lineRenderer);
        }

        #endregion

        #region Logic

        public void DrawCircle(Vector3 fromPosition, float radius, float lifetime = 3)
        {
            _lineRenderer.positionCount = _steps + 1;

            for (int currentStep = 0; currentStep < _steps + 1; currentStep++)
            {
                float circumferenceProgress = (float)currentStep / _steps;
                float currentRadian = circumferenceProgress * 2 * Mathf.PI;

                float xScaled = Mathf.Cos(currentRadian);
                float yScaled = Mathf.Sin(currentRadian);

                float x = xScaled * radius;
                float y = yScaled * radius;

                Vector3 currentPosition = new Vector3(x, y, 0);
                _lineRenderer.SetPosition(currentStep, fromPosition - currentPosition);
            }

            Destroy(gameObject, lifetime);
        }

        #endregion
    }
}