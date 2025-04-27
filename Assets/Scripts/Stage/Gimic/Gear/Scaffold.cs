using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaffold : MonoBehaviour
{
    private Quaternion initialRotation;
    private void Start()
    {
        // 最初のローカル回転を保存
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // 親の回転を打ち消して、最初の向きに固定
        transform.rotation = initialRotation;
    }
}
