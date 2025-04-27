using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageSceneController))]
public class StageSceneControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 通常のInspectorも表示

        StageSceneController script = (StageSceneController)target;
        if (GUILayout.Button("Player Dead"))
        {
            script.SetPlayerDead();
        }

        if (GUILayout.Button("Time Over"))
        {
            script.SetTimeOver();
        }

        if (GUILayout.Button("Game Clear"))
        {
            script.SetGameClear();
        }

        if (GUILayout.Button("Next Stage"))
        {
            StageManager.Instance.NextStage();
            script.SetPlayerDead();
        }
    }
}
