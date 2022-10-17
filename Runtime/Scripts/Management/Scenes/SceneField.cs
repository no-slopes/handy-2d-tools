using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace H2DT.Management.Scenes
{
    /// <summary>
    /// This is a solution to working with scene assets throug Unity's inspector.
    /// It was originally found on a comment by "glitchers" at https://answers.unity.com/questions/242794/inspector-field-for-scene-asset.html#answer-1204071
    /// I moved the property drawer solution into an editor folder.
    /// </summary>
    [System.Serializable]
    public class SceneField
    {
        [SerializeField]
        private Object _sceneAsset;

        [SerializeField]
        private string _sceneName = "";

        public Object sceneAsset
        {
            get { return _sceneAsset; }
            set { _sceneAsset = value; }
        }

        public string sceneName
        {
            get { return _sceneName; }
            set { _sceneName = value; }
        }

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.sceneName;
        }
    }

}
