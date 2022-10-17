using System.Collections.Generic;
using H2DT.Debugging;
using H2DT.Management.Player;
using H2DT.Management.Scenes;
using H2DT.Spawning;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Levels
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class LevelDoorWay : HandyComponent
    {
        #region Inspector 

        [Header("Activation")]
        [Space]
        [SerializeField]
        protected bool _active = true;

        [Header("Door configuration")]
        [Space]
        [SerializeField]
        protected string _code;

        [SerializeField]
        protected LevelInfo _levelInfo;

        [Header("Spawn")]
        [Space]
        [SerializeField]
        protected SpawnInfo _spawnInfo;

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<LevelDoorWay> PlayerPassedThrough;

        #endregion

        #region Fields

        protected BoxCollider2D _boxCollider;
        protected bool _alreadyLeft = false;
        protected bool _loaded = false;

        protected List<GameObject> _targets = new List<GameObject>();

        #endregion

        #region Properties

        public bool loaded => _loaded;

        protected bool canBeUsed => _loaded && _active;

        #endregion

        #region  Getters

        public string code => _code;
        public LevelInfo levelInfo => _levelInfo;
        public SpawnInfo spawnInfo => _spawnInfo;

        #endregion

        #region Mono

        protected void Awake()
        {
            FindComponent<BoxCollider2D>(ref _boxCollider);

            if (string.IsNullOrEmpty(_code))
            {
                Log.Warning($"{gameObject.name} - Door way has null or empty code and will not work.");
            }
        }

        #endregion

        #region Logic

        public void Load()
        {
            _loaded = true;
        }

        protected void EvaluateCollision(GameObject collidedObject)
        {
            if (_alreadyLeft || !canBeUsed) return;

            foreach (GameObject target in _targets)
            {
                if (collidedObject == target)
                {
                    PlayerPassedThrough.Invoke(this);
                    _alreadyLeft = true;
                }

                break;
            }
        }

        public void AddTarget(GameObject target)
        {
            if (!_targets.Contains(target))
            {
                _targets.Add(target);
            }
        }

        public void RemoveTarget(GameObject target)
        {
            _targets.Remove(target);
        }

        #endregion

        #region Collision Callbacks

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            EvaluateCollision(collision.collider.gameObject);
        }

        protected void OnCollisionStay2D(Collision2D collision)
        {
            EvaluateCollision(collision.collider.gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D otherCollider)
        {
            EvaluateCollision(otherCollider.gameObject);
        }

        protected void OnTriggerStay2D(Collider2D otherCollider)
        {
            EvaluateCollision(otherCollider.gameObject);
        }

        #endregion

    }
}
