using UnityEngine;
using UnityEditor;

namespace DancingLineFanmade.Editor
{
    public static class TemplateMenu
    {
        [MenuItem("Template/Tutorial")]
        private static void OpenTutorial()
        {
            Application.OpenURL("https://maxiceflame.github.io/tutorial");
        }

        [MenuItem("Template/MIFT4-7-6Remake")]
        private static void Web()
        {
            Application.OpenURL("https://jokerdontplaydl.github.io/MIFT4-7-6_Remake");
        }
    }
}