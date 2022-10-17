using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using H2DT.Management.Levels;

namespace H2DT.Management.Scenes.Editor
{
    public class SceneGenerator : MonoBehaviour
    {
        [MenuItem("Assets/Create/Handy 2D Tools/Scenes/Generate Scene", false, 1)]
        public static void GenerateScene()
        {
            ShowPrompt();
        }

        private static void ShowPrompt()
        {
            SceneGeneratorPrompt prompt = ScriptableObject.CreateInstance<SceneGeneratorPrompt>();
            prompt.titleContent = new GUIContent("Scene Generator");
        }
    }

    public class SceneGeneratorPrompt : EditorWindow
    {
        bool _createFolder = true;
        string _sceneName;
        bool _alsoGenerateLevelInfo = false;
        string _levelInfoName;
        string _levelName;

        private void OnEnable()
        {
            GetWindow(typeof(SceneGeneratorPrompt));
        }

        private void OnGUI()
        {

            GUILayout.Space(10);

            // Create a folder field
            _createFolder = EditorGUILayout.Toggle("Store in a folder?", _createFolder);

            GUILayout.Space(10);

            // Scene name field
            EditorGUILayout.LabelField("Scene Name");
            _sceneName = EditorGUILayout.TextField(_sceneName);

            GUILayout.Space(10);

            // Generate Level info field
            _alsoGenerateLevelInfo = EditorGUILayout.Toggle("Also generate level info?", _alsoGenerateLevelInfo);

            GUILayout.Space(10);

            if (_alsoGenerateLevelInfo)
            {
                // Scene name field
                EditorGUILayout.LabelField("Level Info Name");
                _levelInfoName = EditorGUILayout.TextField(_levelInfoName);

                GUILayout.Space(10);

                EditorGUILayout.LabelField("Level Name");
                _levelName = EditorGUILayout.TextField(_levelName);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate"))
            {
                ProcessInput();
            }

            GUILayout.Space(10);

            Event e = Event.current;
            if (e.keyCode == KeyCode.Return)
            {
                ProcessInput();
            }
        }

        public void ProcessInput()
        {
            // User chooses path to save scene
            // string path = EditorUtility.OpenFolderPanel("Where to Generate", Application.dataPath, "");
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (_createFolder)
            {
                AssetDatabase.CreateFolder(path, _sceneName);
                path += "/" + _sceneName;
            }

            // Create string paths
            string scriptableScenePath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + _sceneName + ".asset");
            string scenePath = path + "/" + _sceneName + ".unity";

            // Creates the scene, saves it and unloads it from hierarchy
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            EditorSceneManager.SaveScene(scene, scenePath);
            EditorSceneManager.UnloadSceneAsync(scene);

            // Gets the scene asset from project folder
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)) as SceneAsset;

            // Creates the scriptable object 
            SceneInfo sceneScriptableObject = ScriptableObject.CreateInstance<SceneInfo>();
            sceneScriptableObject.GenerateId();

            // Sets the just saved scene asset into the scene info scriptable object
            SceneField sceneField = new SceneField();
            sceneField.sceneAsset = sceneAsset;
            sceneField.sceneName = sceneAsset.name;

            sceneScriptableObject.sceneField = sceneField;

            // Creates the scriptable object saving it on the given path
            AssetDatabase.CreateAsset(sceneScriptableObject, scriptableScenePath);

            if (_alsoGenerateLevelInfo)
            {
                // Creates the scriptable object 
                LevelInfo levelInfoScriptableObject = ScriptableObject.CreateInstance<LevelInfo>();
                levelInfoScriptableObject.GenerateId();
                levelInfoScriptableObject.scene = sceneScriptableObject;
                levelInfoScriptableObject.levelName = _levelName;

                // Creating the asset
                string scriptablLevelPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + _levelInfoName + ".asset");
                AssetDatabase.CreateAsset(levelInfoScriptableObject, scriptablLevelPath);
            }

            // Saves all assets
            AssetDatabase.SaveAssets();

            // Refreshes the project window and close the Scene Generator window
            AssetDatabase.Refresh();
            this.Close();
        }
    }
}