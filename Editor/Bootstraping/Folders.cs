using UnityEngine;
using UnityEditor;
using System.IO;

namespace H2DT.Editor.BootStraping
{
    public static class Folders
    {
        [MenuItem("Tools/H2DT/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            CreateDirectories("_Project", "Scripts", "Scenes", "Management", "Graphics", "Lozalization");
            AssetDatabase.Refresh();
        }

        public static void CreateDirectories(string root, params string[] directoriesNames)
        {
            string fullPath = Path.Combine(Application.dataPath, root);

            foreach (string directoryName in directoriesNames)
            {
                Directory.CreateDirectory(Path.Combine(fullPath, directoryName));
            }
        }
    }
}
