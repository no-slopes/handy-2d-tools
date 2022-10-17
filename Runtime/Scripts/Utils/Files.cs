using System.IO;

namespace H2DT.Utils
{
    public static class Files
    {
        /// <summary>
        /// Generates a GUID verifying if any file exists using that ID
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GenerateGuidForFilename(string directoryPath, string extension)
        {
            string guid;

            do
            {
                guid = System.Guid.NewGuid().ToString();
            }
            while (File.Exists($"{directoryPath}/{guid}{extension}"));

            return guid;
        }
    }
}