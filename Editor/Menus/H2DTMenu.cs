using UnityEngine;
using UnityEditor;
using System.IO;

namespace DIDI.Editor
{
    public static class H2DTMenu
    {
        [MenuItem("Tools/H2DT/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            CreateDirectories("_Props", "Scripts", "Scenes", "Management", "Graphics", "Lozalization");
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
