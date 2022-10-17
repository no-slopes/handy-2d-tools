using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using H2DT;
using H2DT.Debugging;
using UnityEngine;

namespace H2DT.Management.Levels
{
    public class LevelCameraManager : HandyComponent
    {
        #region Inspector

        [Header("Cameras")]
        [Space]
        [SerializeField]
        protected List<CinemachineVirtualCamera> _playerCameras = new List<CinemachineVirtualCamera>();

        #endregion

        #region Mono

        protected void Awake()
        {

        }

        #endregion

        #region Logic

        protected void InitializeConfiners()
        {

        }

        protected void DefineCameraConfiner(CinemachineVirtualCamera camera, PolygonCollider2D confiner)
        {
            CinemachineConfiner2D confinerComponent = camera.GetComponent<CinemachineConfiner2D>();

            if (confinerComponent == null)
            {
                Log.Warning($"{gameObject.name} - Trying to set confiner for {camera.gameObject.name} but no confiner component attached into it.");
                return;
            }

            confinerComponent.m_BoundingShape2D = confiner;
        }

        public void SetVirtualCamerasPlayer(GameObject player)
        {
            _playerCameras.ForEach(playerCamera => playerCamera.Follow = player.transform);
        }

        #endregion
    }
}
