using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageSceneController))]
public class StageSceneControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 通常のInspectorも表示

        if (GUILayout.Button("Game Over"))
        {
            StageManager.Instance.SetGameOver();
        }

        if (GUILayout.Button("Game Clear"))
        {
            StageManager.Instance.SetGameClear();
        }
    }
}
