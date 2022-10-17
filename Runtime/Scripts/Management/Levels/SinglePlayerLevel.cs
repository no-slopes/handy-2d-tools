using System;
using UnityEngine;
using H2DT;
using H2DT.Spawning;
using H2DT.Debugging;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Management.Player;

namespace H2DT.Management.Levels
{
    public abstract class SinglePlayerLevel : Level
    {
        #region Inspector        

        [Header("Player Spawning")]
        [Space]
        [SerializeField]
        private bool _shouldSpawnPlayer = true;

        [SerializeField]
        private bool _spawnPlayerAsChild = false;

        [Space]
        [SerializeField]
        private SpawnInfo _defaultSpawnInfo;

        [SerializeField]
        private List<LevelDoorWay> _doorWays = new List<LevelDoorWay>();

        #endregion

        #region Fields

        private GameObject _player;
        private LevelCameraManager _levelCameraManager;
        private PlayerHandler _playerHandler;

        #endregion

        #region Properties

        #endregion

        #region Getters

        #endregion        

        #region Mono

        public override async Task Initialize()
        {
            if (_levelCameraManager == null)
                FindComponent<LevelCameraManager>(ref _levelCameraManager);

            await base.Initialize();
        }

        protected void OnEnable()
        {
            _doorWays.ForEach(doorWay => doorWay.PlayerPassedThrough.AddListener(OnPlayerPassedThroughDoorWay));
        }

        protected void OnDisable()
        {
            _doorWays.ForEach(doorWay => doorWay.PlayerPassedThrough.RemoveListener(OnPlayerPassedThroughDoorWay));
        }

        #endregion

        #region Level Logic

        public async Task LeaveThroughDoor(LevelDoorWay doorWay)
        {
            await Leave(doorWay.levelInfo, doorWay.code);
        }

        #endregion

        #region Level Doors        

        protected void LoadDoors()
        {
            _doorWays.ForEach(doorWay => doorWay.Load());
        }

        protected async void OnPlayerPassedThroughDoorWay(LevelDoorWay doorWay)
        {
            await LeaveThroughDoor(doorWay);
        }

        #endregion

        #region Level Callbacks

        protected void AfterLoad()
        {
            HandlePlayerSpawn();
        }

        protected void OnApperaingOnScreen()
        {
            LoadDoors();
        }

        #endregion

        #region Spawn Logic

        protected void HandlePlayerSpawn()
        {
            if (!ValidatePlayerSpawn()) return;

            string doorCode = levelHandler.GetPreviousLevelTrail<string>();

            if (!string.IsNullOrEmpty(doorCode))
            {
                _player = SpawnAtDoorWay(_doorWays, doorCode);
            }
            else
            {
                _player = SpawnAtDefaultSpawnPoint();
            }

            if (_player == null) return;

            sublevelAnchors.ForEach(anchor => anchor.AddTarget(_player));
        }

        protected bool ValidatePlayerSpawn()
        {
            if (!_shouldSpawnPlayer) return false;

            if (_playerHandler == null)
            {
                Log.Danger($"{gameObject.name} - Should spawn player but playerManager is null");
                return false;
            }

            return true;
        }

        public GameObject SpawnAtDefaultSpawnPoint()
        {
            if (_defaultSpawnInfo == null)
            {
                Log.Danger($"{gameObject.name} - Trying to Spawn Player at null spawn point.");
                return null;
            }

            return SpawnPlayer(_defaultSpawnInfo);
        }

        public GameObject SpawnAtDoorWay(List<LevelDoorWay> doorWays, string lastUsedDoorWayCode)
        {
            LevelDoorWay doorWayToSpawnAt = doorWays.Find(doorWay => doorWay.code == lastUsedDoorWayCode);

            if (doorWayToSpawnAt != null)// Case last doorway code do not match any of our codes.
            {
                return SpawnPlayer(doorWayToSpawnAt.spawnInfo);
            }
            else
            {
                Log.Danger($"{gameObject.name} - Trying to Spawn Player at door way but code do not match any doors in list");
                return null;
            }
        }

        public GameObject SpawnPlayer(SpawnInfo spawnInfo)
        {
            Transform parent = _spawnPlayerAsChild ? transform : null;
            GameObject player = _playerHandler.SpawnPlayer(spawnInfo, parent);

            _doorWays.ForEach(doorWay => doorWay.AddTarget(player));
            sublevelAnchors.ForEach(sublevelAnchor => sublevelAnchor.AddTarget(player));
            UpdatePlayerForCameras(player);

            return player;
        }

        #endregion

        #region  Cameras

        protected void UpdatePlayerForCameras(GameObject player)
        {
            if (player == null) return;

            _levelCameraManager?.SetVirtualCamerasPlayer(player);
        }

        #endregion
    }
}