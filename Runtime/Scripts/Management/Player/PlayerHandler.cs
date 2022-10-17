using System.Collections;
using System.Collections.Generic;
using H2DT.Actions;
using H2DT.Enums;
using H2DT.Spawning;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace H2DT.Management.Player
{
    [DefaultExecutionOrder(-6000)]
    public abstract class PlayerHandler : ScriptableObject
    {
        #region Inspector

        [Header("Players")]
        [Space]
        [SerializeField]
        protected GameObject _defaultPrefab;

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<GameObject> PlayerSpawned;

        [SerializeField]
        public UnityEvent<GameObject> PlayerRemoved;

        #endregion

        #region Abstractions  

        public abstract GameObject SpawnPlayer(GameObject prefab, SpawnInfo spawnInfo, Transform parent = null);
        public abstract GameObject SpawnPlayer(SpawnInfo spawnInfo, Transform parent = null);
        public abstract void RemovePlayer(GameObject player);

        #endregion
    }
}
