using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace USP.MiniGame.recognitionPatterns
{
    /// <summary>
    /// Script by Vishal Lakhani
    /// Email: usp.vishal@gmail.com
    /// Description:
    /// </summary>
    public class AudioEnumGenerator
    {
#if UNITY_EDITOR
        private const string path = "Assets/Generated/AudioEnum.cs";

        [MenuItem("Tools/Generate Audio Enum")]
        public static void GenerateEnum()
        {
            AudioLibrary lib = GameObject.FindObjectOfType<AudioLibrary>();

            if (lib == null)
            {
                Debug.LogError("AudioLibrary not found in scene!");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("public enum AudioID");
            sb.AppendLine("{none,");

            foreach (var audio in lib.audioLib)
            {
                if (string.IsNullOrEmpty(audio.name)) continue;

                string safeName = audio.name.Replace(" ", "_");
                if (char.IsDigit(safeName[0]))
                {
                    safeName = "_" + safeName;
                }
                sb.AppendLine($"    {safeName},");
            }

            sb.AppendLine("}");

            // Ensure folder exists
            Directory.CreateDirectory("Assets/Generated");

            File.WriteAllText(path, sb.ToString());

            AssetDatabase.Refresh();

            Debug.Log("Audio Enum Generated!");

        }
#endif
    }

}