using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using H2DT.Debugging;
using H2DT.Enums;
using H2DT.Spawning;

namespace H2DT.Management.Player
{
    [CreateAssetMenu(fileName = "New Single Player Handler", menuName = "Handy 2D Tools/Management/Players/Single Player Handler")]
    public class SinglePlayerHandler : PlayerHandler
    {
        #region Fields

        protected GameObject _player;
        protected PlayerInput _playerInput;

        #endregion

        #region Getters

        public GameObject player => _player;
        public PlayerInput playerInput => _playerInput;

        #endregion

        #region Logic

        public override GameObject SpawnPlayer(GameObject prefab, SpawnInfo spawnInfo, Transform parent = null)
        {
            if (_player != null)
                Destroy(_player.gameObject);

            _player = Instantiate(prefab, spawnInfo.point.position, Quaternion.identity, parent);
            _playerInput = _player.GetComponent<PlayerInput>();

            if (_playerInput == null)
            {
                Destroy(_player);
                Log.Danger($"{_player.name} does not have a PlayerInput component associated with it.");
                return null;
            }

            if (spawnInfo != null)
            {
                ISpawnSubject spawnSubject = _player.GetComponent<ISpawnSubject>();
                spawnSubject?.OnSpawn(spawnInfo);
            }

            PlayerSpawned.Invoke(_player);
            return _player;
        }

        public override GameObject SpawnPlayer(SpawnInfo spawnInfo, Transform parent = null)
        {
            return SpawnPlayer(_defaultPrefab, spawnInfo, parent);
        }

        public override void RemovePlayer(GameObject player)
        {
            if (player != _player) return;

            PlayerRemoved.Invoke(_player);
            Destroy(_player.gameObject);
            _player = null;
        }

        public void RemovePlayer()
        {
            RemovePlayer(_player);
        }

        #endregion

        #region Player Input Callbacks

        protected void OnInputUserChanged(InputUser inputUser, InputUserChange change, InputDevice device)
        {
            if (inputUser == _playerInput?.user)
            {
                // TODO: Change input visuals
            }
        }

        #endregion
    }
}
