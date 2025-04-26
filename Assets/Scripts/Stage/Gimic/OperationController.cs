using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OperationController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_rightText = null;
    [SerializeField] private TextMeshProUGUI m_leftText = null;
    [SerializeField] private TextMeshProUGUI m_jumpText = null;
    [SerializeField] private TextMeshProUGUI m_crouchText = null;

    public void SetText()
    {
        m_rightText.text = m_rightText.text;
        m_leftText.text = m_leftText.text;
        m_jumpText.text = m_jumpText.text;
        m_crouchText.text = m_crouchText.text;
    }
}
