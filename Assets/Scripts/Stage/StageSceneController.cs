using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSceneController : MonoBehaviour
{
    void Start()
    {
        // Stage開始
        StageManager.Instance.PlayStart();
    }
}
