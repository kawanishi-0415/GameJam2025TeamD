using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 50.0f;

    private void Update()
    {
        transform.eulerAngles += new Vector3(0f, 0f, m_rotationSpeed * Time.deltaTime);
    }
}
