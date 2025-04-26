using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_StageDatabase", menuName = "Game/SO_StageDatabase", order = 2)]
public class SO_StageDatabase : ScriptableObject
{
    public List<SO_StageData> stageList;
}
