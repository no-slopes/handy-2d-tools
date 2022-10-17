using System.Collections;
using System.Collections.Generic;
using H2DT.Enums;
using H2DT.Spawning;
using UnityEngine;
using UnityEngine.InputSystem;

namespace H2DT.Management.Player
{
    // [RequireComponent(typeof(PlayerInputManager))]
    // public class LocalMultiPlayerHandler : PlayerHandler
    // {
    //     #region Fields

    //     protected PlayerInputManager _playerInputManager;
    //     protected List<PlayerInput> _players = new List<PlayerInput>();

    //     #endregion

    //     #region Getters

    //     public PlayerInputManager playerInputManager => _playerInputManager;

    //     #endregion


    //     #region Logic

    //     public override GameObject SpawnPlayer(GameObject prefab, Vector3 spawnPoint, Transform parent = null, SpawnInfo spawnInfo = null)
    //     {

    //         return PlayerInput.Instantiate(prefab).gameObject;
    //     }

    //     public override GameObject SpawnPlayer(Vector3 spawnPoint, Transform parent = null, SpawnInfo spawnInfo = null)
    //     {

    //         return SpawnPlayer(_defaultPrefab, spawnPoint, parent);
    //     }

    //     public override void RemovePlayer(GameObject player)
    //     {

    //         RemovePlayer();
    //     }

    //     public void RemovePlayer()
    //     {
    //     }

    //     #endregion

    // }
}
