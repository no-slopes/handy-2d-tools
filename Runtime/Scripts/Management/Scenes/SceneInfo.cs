using System.Collections;
using System.Collections.Generic;
using H2DT.Management.Levels;
using H2DT.NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace H2DT.Management.Scenes
{
    [CreateAssetMenu(fileName = "Scene Info", menuName = "Handy 2D Tools/Scenes/Scene Info")]
    public class SceneInfo : ScriptableObject
    {
        #region  Inspector

        [Space]
        [ReadOnly]
        [SerializeField]
        private string _id;

        [Header("General")]
        [Space]
        [SerializeField]
        protected SceneField _sceneField;

        [Header("Exit Transition")]
        [Space]
        [SerializeField]
        protected GameObject _enterTransitionPrefab;

        #endregion

        #region Fields


        #endregion

        #region Properties

        public SceneField sceneField { get => _sceneField; set => _sceneField = value; }

        #endregion

        #region Getters

        // Values
        public string id => _id;

        // Transition
        public GameObject enterTransitionPrefab => _enterTransitionPrefab;

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
