using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOperationObj : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StageSceneController.Instance.ChangeOperation();
        Destroy(gameObject);
    }
}
