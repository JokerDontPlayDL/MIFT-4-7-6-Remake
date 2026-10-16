using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Template._Scripts_.Editor
{
    public class Tool_FastScreenShot : EditorWindow
    {
        private static string directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\Screen Shot";
        private static string latestScreenshotPath = "";

        [MenuItem("PIXEL/Screen Capture/�򿪴洢�ļ��� &2", false, 2)]
        private static void ShowFolder()
        {
            if (File.Exists(latestScreenshotPath))
            {
                EditorUtility.RevealInFinder(latestScreenshotPath);
                return;
            }

            Directory.CreateDirectory(directory);
            EditorUtility.RevealInFinder(directory);
        }

        [MenuItem("PIXEL/Screen Capture/��ͼ &1", false, 1)]
        private static void TakeScreenshot()
        {
            Directory.CreateDirectory(directory);
            var currentTime = System.DateTime.Now;
            var filename = currentTime.ToString().Replace('/', '-').Replace(':', '_') + ".png";
            var path = directory + "\\" + filename;
            ScreenCapture.CaptureScreenshot(path);
            latestScreenshotPath = path;
            Debug.Log($"��ͼ·��: <b>{path}</b> �ֱ��ʣ� <b>{GetResolution()}</b>");
        }

        private static string GetResolution()
        {
            Vector2 size = Handles.GetMainGameViewSize();
            Vector2Int sizeInt = new Vector2Int((int)size.x, (int)size.y);
            return $"{sizeInt.x}x{sizeInt.y}";
        }
    }
}
