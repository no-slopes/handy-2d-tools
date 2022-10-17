using UnityEngine;
using System.IO;
using H2DT.Debugging;
using H2DT.NaughtyAttributes;

namespace H2DT.Utils
{
    public class ScreenShooter : MonoBehaviour
    {
        [Button]
        public void Shoot()
        {
            string currentTime = System.DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");

            try
            {
                string screenshotDir = Application.dataPath + "/../Screenshots";

                if (!Directory.Exists(screenshotDir))
                    Directory.CreateDirectory(screenshotDir);

                ScreenCapture.CaptureScreenshot($"{screenshotDir}/{currentTime}.png");
                Log.Success($"Screenshot saved at {screenshotDir}/{currentTime}.png");
            }
            catch (System.Exception)
            {
                Log.Danger($"Screenshot failed");
            }
        }
    }
}