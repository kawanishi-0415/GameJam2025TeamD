using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoleObj : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StageManager.Instance.SetGameClear();
    }
}
