using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageSceneController : MonoBehaviour
{
    [SerializeField] private Vector3 m_startCameraPos = new Vector3(0f, 0f, -10f);
    [SerializeField] private TextMeshProUGUI m_stageText = null;
    [SerializeField] private TextMeshProUGUI m_timeText = null;

    private Camera m_camera = null;

    void Start()
    {
        // Stage開始
        m_camera = Camera.main;
        m_camera.transform.position = m_startCameraPos;
        StageManager.Instance.PlayStart();
    }

    private void Update()
    {
        // スクロール
        MoveCamera();

        // UI表示
        DispStageText();
        DispTimeText();
    }

    private void DispStageText()
    {
        m_stageText.enabled = StageManager.Instance.Status == StageManager.EnumStageStatus.Playing;
        m_stageText.text = StageManager.Instance.StageName;
    }

    private void DispTimeText()
    {
        m_timeText.enabled = StageManager.Instance.Status == StageManager.EnumStageStatus.Playing;
        m_timeText.text = $"Time Limit: {Mathf.CeilToInt(StageManager.Instance.TimeLimit)}";
    }

    private void MoveCamera()
    {
        if (StageManager.Instance.Status != StageManager.EnumStageStatus.Playing) return;
        m_camera.transform.position += new Vector3(StageManager.Instance.ScrollSpeed * Time.deltaTime, 0f, 0f);
    }
}
