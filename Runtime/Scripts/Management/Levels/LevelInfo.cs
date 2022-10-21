using H2DT.Management.Scenes;
using H2DT.NaughtyAttributes;
using UnityEngine;

namespace H2DT.Management.Levels
{
    [CreateAssetMenu(fileName = "Level Info", menuName = "Handy 2D Tools/Levels/Level Info")]
    public class LevelInfo : ScriptableObject
    {
        #region Inspector

        [Header("ID")]
        [Space]
        [ReadOnly]
        [SerializeField]
        private string _id;

        [Header("Level")]
        [Space]
        [SerializeField]
        private string _levelName;

        [Header("Scene")]
        [Space]
        [SerializeField]
        private SceneInfo _scene;

        [Space]
        [SerializeField]
        protected bool _mustInstantiate;

        [ShowIf("_mustInstantiate")]
        [SerializeField]
        protected GameObject _levelPrefab;

        #endregion

        #region Fields

        #endregion

        #region  Getters

        public string id => _id;

        public SceneInfo scene { get => _scene; set => _scene = value; }
        public string levelName { get => _levelName; set => _levelName = value; }
        public bool mustInstantiate { get => _mustInstantiate; set => _mustInstantiate = value; }
        public GameObject levelPrefab { get => _levelPrefab; set => _levelPrefab = value; }

        #endregion        

        #region ID

        [ContextMenu("Generate ID")]
        public void GenerateId()
        {
            _id = System.Guid.NewGuid().ToString();
        }

        #endregion
    }
}
