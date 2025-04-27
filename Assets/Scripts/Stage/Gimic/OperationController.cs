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
    [SerializeField] private TextMeshProUGUI m_changeTime = null;

    private void Update()
    {
        
    }

    public void SetText(string rightText, string leftText, string jumpText, string crouchText)
    {
        m_rightText.text = rightText;
        m_leftText.text = leftText;
        m_jumpText.text = jumpText;
        m_crouchText.text = crouchText;
    }

    public void ChangeTimeText()
    {
        m_changeTime.text = Mathf.CeilToInt(StageSceneController.Instance.ChangeTime).ToString();
    }
}
