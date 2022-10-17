using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

namespace H2DT.Actions.Editor
{
    public class FSMSetuper : MonoBehaviour
    {
        [MenuItem("Assets/Create/Handy 2D Tools/FSM/Set up", false, 1)]
        public static void SetUp()
        {
            ShowPrompt();
        }

        private static void ShowPrompt()
        {
            FSMGeneratorPrompt prompt = ScriptableObject.CreateInstance<FSMGeneratorPrompt>();
            prompt.titleContent = new GUIContent("Set up FSM");
        }
    }

    public class FSMGeneratorPrompt : EditorWindow
    {
        static string TemplatesFolderPath = "Editor/Handy 2D Tools/FSM Templates";
        static string ActorTemplateFileName = "ActorTemplate.txt";
        static string ActorFSMTemplateFileName = "ActorFSMTemplate.txt";
        static string ActorStateTemplateFileName = "ActorStateTemplate.txt";

        bool _createFolder = false;

        string _namespaceText;
        string _actorName;

        private void OnEnable()
        {
            GetWindow(typeof(FSMGeneratorPrompt));
        }

        private void OnGUI()
        {

            GUILayout.Space(10);

            // Create a folder field
            _createFolder = EditorGUILayout.Toggle("Store inside a folder?", _createFolder);

            GUILayout.Space(10);

            // Namespace field
            EditorGUILayout.LabelField("Namespace");
            _namespaceText = EditorGUILayout.TextField(_namespaceText);

            // Actor name field
            EditorGUILayout.LabelField("Actor Name");
            _actorName = EditorGUILayout.TextField(_actorName);

            GUILayout.Space(10);

            if (GUILayout.Button("Generate"))
            {
                ProcessInput(_namespaceText, _actorName);
            }

            GUILayout.Space(10);

            Event e = Event.current;
            if (e.keyCode == KeyCode.Return)
            {
                ProcessInput(_namespaceText, _actorName);
            }
        }

        public void ProcessInput(string namespaceText, string actorName)
        {
            // User chooses path to save FSM
            // string path = EditorUtility.OpenFolderPanel("Where to Generate", Application.dataPath, "");
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (_createFolder)
            {
                AssetDatabase.CreateFolder(path, actorName);
                path += "/" + actorName;
            }

            // Creating the Actor script
            string actorFilePath = path + "/" + _actorName + ".cs";
            CreateFile(actorFilePath, $"{Application.dataPath}/{TemplatesFolderPath}/{ActorTemplateFileName}");

            // Creating the Actor FSM script
            string actorFSMFilePath = path + "/" + _actorName + "FSM.cs";
            CreateFile(actorFSMFilePath, $"{Application.dataPath}/{TemplatesFolderPath}/{ActorFSMTemplateFileName}");

            // Creating the Actor State script
            string actorStateFilePath = path + "/" + _actorName + "State.cs";
            CreateFile(actorStateFilePath, $"{Application.dataPath}/{TemplatesFolderPath}/{ActorStateTemplateFileName}");

            // Saves all assets
            AssetDatabase.SaveAssets();

            // Refreshes the project window and close the window
            AssetDatabase.Refresh();
            this.Close();
        }

        private void CreateFile(string filePath, string templatePath)
        {
            try
            {
                string templateContents = File.ReadAllText(templatePath);
                string fileContents = templateContents.Replace("{actor}", _actorName).Replace("{namespace}", _namespaceText);
                File.WriteAllTextAsync(filePath, fileContents);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Unable to create file <b>{filePath}</b> using the <b>{templatePath}</b> template. Are you sure the FSM Template files are under <b>{TemplatesFolderPath}</b>?");
                Debug.LogError(e.Message);
            }
        }

    }
}