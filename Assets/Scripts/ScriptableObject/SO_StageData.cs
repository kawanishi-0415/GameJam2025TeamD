using UnityEngine;

[CreateAssetMenu(fileName = "SO_StageData", menuName = "Game/SO_StageData", order = 1)]
public class SO_StageData : ScriptableObject
{
    public string stageName = "Stage1";
    public Vector2 playerStartPosition = new Vector2(0f, 2f);
    public float scrollSpeed = 1f;
    public float timeLimit = 999f;
    public GameObject stagePrefab = null;
}
