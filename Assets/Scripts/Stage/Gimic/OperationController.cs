using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperationController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_rightText = null;
    [SerializeField] private TextMeshProUGUI m_leftText = null;
    [SerializeField] private TextMeshProUGUI m_jumpText = null;
    [SerializeField] private TextMeshProUGUI m_crouchText = null;

    [SerializeField] private Image m_rightFrame = null;
    [SerializeField] private Image m_leftFrame = null;
    [SerializeField] private Image m_jumpFrame = null;
    [SerializeField] private Image m_crouchFrame = null;

    [SerializeField] private TextMeshProUGUI m_rightTitleText = null;
    [SerializeField] private TextMeshProUGUI m_leftTitleText = null;
    [SerializeField] private TextMeshProUGUI m_jumpTitleText = null;
    [SerializeField] private TextMeshProUGUI m_crouchTitleText = null;

    [SerializeField] private TextMeshProUGUI m_changeTime = null;

    private Dictionary<KeyCode, Color> m_textColorTable = new Dictionary<KeyCode, Color>() {
        { KeyCode.Q, new Color(0.235f, 1f, 0f, 1f) },
        { KeyCode.R, new Color(1f, 0.506f, 0.506f, 1f) },
        { KeyCode.U, new Color(0.318f, 0.71f, 1f, 1f) },
        { KeyCode.P, new Color(1f, 0.965f, 0.365f, 1f) },
};

    private void Update()
    {
        ChangeTimeText();
    }

    public void SetText(KeyCode rightKey, KeyCode leftKey, KeyCode jumpKey, KeyCode crouchKey)
    {
        m_rightText.text = rightKey.ToString();
        if (m_textColorTable.ContainsKey(rightKey))
        {
            m_rightText.color = m_textColorTable[rightKey];
            m_rightFrame.color = m_textColorTable[rightKey];
            m_rightTitleText.color = m_textColorTable[rightKey];
        }

        m_leftText.text = leftKey.ToString();
        if (m_textColorTable.ContainsKey(leftKey))
        {
            m_leftText.color = m_textColorTable[leftKey];
            m_leftFrame.color = m_textColorTable[leftKey];
            m_leftTitleText.color = m_textColorTable[leftKey];
        }

        m_jumpText.text = jumpKey.ToString();
        if (m_textColorTable.ContainsKey(jumpKey))
        {
            m_jumpText.color = m_textColorTable[jumpKey];
            m_jumpFrame.color = m_textColorTable[jumpKey];
            m_jumpTitleText.color = m_textColorTable[jumpKey];
        }

        m_crouchText.text = crouchKey.ToString();
        if (m_textColorTable.ContainsKey(crouchKey))
        {
            m_crouchText.color = m_textColorTable[crouchKey];
            m_crouchFrame.color = m_textColorTable[crouchKey];
            m_crouchTitleText.color = m_textColorTable[crouchKey];
        }
    }

    public void ChangeTimeText()
    {
        m_changeTime.text = Mathf.CeilToInt(StageSceneController.Instance.ChangeTime).ToString();
    }
}
