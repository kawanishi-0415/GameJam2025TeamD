using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class UniqueCharacterExtractor : EditorWindow
{
    private List<TextAsset> targetTextFiles = new List<TextAsset>();
    private TextAsset saveTextFile;
    private string result = "";

    [MenuItem("Tools/Unique Character Extractor")]
    public static void OpenWindow()
    {
        GetWindow<UniqueCharacterExtractor>("文字抽出ツール");
    }

    private void OnGUI()
    {
        GUILayout.Label("テキストファイルを選択", EditorStyles.boldLabel);

        int targetFileCount = Mathf.Max(1, EditorGUILayout.IntField("ターゲットファイル数", targetTextFiles.Count));
        while (targetTextFiles.Count < targetFileCount)
        {
            targetTextFiles.Add(null);
        }
        while (targetTextFiles.Count > targetFileCount)
        {
            targetTextFiles.RemoveAt(targetTextFiles.Count - 1);
        }

        for (int i = 0; i < targetTextFiles.Count; i++)
        {
            targetTextFiles[i] = (TextAsset)EditorGUILayout.ObjectField($"ターゲットファイル{i + 1}", targetTextFiles[i], typeof(TextAsset), false);
        }

        GUILayout.Label("保存ファイルを選択", EditorStyles.boldLabel);
        saveTextFile = (TextAsset)EditorGUILayout.ObjectField("保存ファイル", saveTextFile, typeof(TextAsset), false);

        if (GUILayout.Button("文字を抽出して保存する"))
        {
            ExtractAndSaveUniqueCharacters();
        }

        GUILayout.Space(10);
        GUILayout.Label("結果 (重複なし):", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(result, GUILayout.Height(100));
    }

    private void ExtractAndSaveUniqueCharacters()
    {
        if (targetTextFiles == null || targetTextFiles.Count == 0 || targetTextFiles.Any(t => t == null))
        {
            Debug.LogWarning("すべてのターゲットファイルを設定してください！");
            return;
        }

        if (saveTextFile == null)
        {
            Debug.LogWarning("保存ファイルを設定してください！");
            return;
        }

        string saveFilePath = AssetDatabase.GetAssetPath(saveTextFile);

        HashSet<char> uniqueChars = new HashSet<char>();

        foreach (var textAsset in targetTextFiles)
        {
            foreach (char c in textAsset.text)
            {
                if (!char.IsWhiteSpace(c))
                {
                    uniqueChars.Add(c);
                }
            }
        }

        if (File.Exists(saveFilePath))
        {
            string existingContent = File.ReadAllText(saveFilePath);
            foreach (char c in existingContent)
            {
                uniqueChars.Add(c);
            }
        }

        result = new string(uniqueChars.OrderBy(c => c).ToArray());

        File.WriteAllText(saveFilePath, result);

        Debug.Log("文字抽出・保存完了！" + saveFilePath);
        AssetDatabase.Refresh();
    }
}
