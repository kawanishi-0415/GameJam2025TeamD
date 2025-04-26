using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageManager))]
public class StageManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 通常のInspectorも表示

        StageManager script = (StageManager)target;
        if (GUILayout.Button("Game Over"))
        {
            script.SetGameOver();
        }

        if (GUILayout.Button("Game Clear"))
        {
            script.SetGameClear();
        }
    }
}
