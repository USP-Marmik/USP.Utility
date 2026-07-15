using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public static class USPScriptCreator
{
    private const string TemplatePath = "Assets/_DONOTIMPORTTOANOTHERGAME/Templates/USPMonoBehaviourTemplate.txt";

    private const string DefaultNamespace = "USP.MiniGame.recognitionPatterns"; // change this first

    [MenuItem("Assets/Create/Scripting/New MonoBehaviour NameSpaced", false, 80)]
    public static void CreateScript()
    {
        string template = LoadTemplate();

        if (string.IsNullOrEmpty(template))
        {
            EditorUtility.DisplayDialog(
                "Template Missing",
                $"Could not find template at:\n{TemplatePath}",
                "OK");
            return;
        }

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<CreateScriptAssetAction>(),
            "NewMonoBehaviourScript.cs",
            EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
            null);
    }

    private static string LoadTemplate()
    {
        if (!File.Exists(TemplatePath))
            return null;

        return File.ReadAllText(TemplatePath);
    }

    private class CreateScriptAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string template = LoadTemplate();

            if (string.IsNullOrEmpty(template))
            {
                EditorUtility.DisplayDialog(
                    "Template Missing",
                    $"Could not find template at:\n{TemplatePath}",
                    "OK");
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(pathName);
            string namespaceName = GenerateNamespaceFromPath(pathName);

            string scriptText = template
                .Replace("#SCRIPTNAME#", fileName)
                .Replace("#NAMESPACE#", namespaceName);

            File.WriteAllText(pathName, scriptText);
            AssetDatabase.ImportAsset(pathName);

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }

        private static string GenerateNamespaceFromPath(string assetPath)
        {
            string folderPath = Path.GetDirectoryName(assetPath)?.Replace("\\", "/") ?? string.Empty;

            if (string.IsNullOrEmpty(folderPath))
                return DefaultNamespace;

            string[] parts = folderPath.Split('/');
            System.Collections.Generic.List<string> namespaceParts = new System.Collections.Generic.List<string>();

            namespaceParts.Add(DefaultNamespace);

            bool afterScriptsFolder = false;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                if (part == "Assets" || part == "Editor" || part == "Templates")
                    continue;

                if (part == "Scripts")
                {
                    afterScriptsFolder = true;
                    continue;
                }

                if (!afterScriptsFolder)
                    continue;

                string cleaned = CleanNamespacePart(part);
                if (!string.IsNullOrEmpty(cleaned))
                    namespaceParts.Add(cleaned);
            }

            return string.Join(".", namespaceParts);
        }

        private static string CleanNamespacePart(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            }

            string result = sb.ToString();

            if (string.IsNullOrEmpty(result))
                return string.Empty;

            if (char.IsDigit(result[0]))
                result = "_" + result;

            return result;
        }
    }
}