using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class StageSceneController : MonoBehaviour
{
    public static StageSceneController Instance { get; private set; }

    private Camera m_camera = null;
    private PlayerController m_playerObj = null;

    [SerializeField] private Vector3 m_startCameraPos = new Vector3(0f, 0f, -10f);
    [SerializeField] private TextMeshProUGUI m_stageText = null;
    [SerializeField] private TextMeshProUGUI m_timeText = null;
    [SerializeField] private PlayerController m_playerPrefab = null;
    public PlayerController PlayerPrefab { get { return m_playerPrefab; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

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

    public void CreatePlayer(Vector2 startPosition)
    {
        if (m_playerObj != null)
            Destroy(m_playerObj.gameObject);

        m_playerObj = Instantiate(StageSceneController.Instance.PlayerPrefab);
        m_playerObj.transform.position = startPosition;
    }

    public void ChangeOperation()
    {
    }

    private void DispStageText()
    {
        m_stageText.enabled = StageManager.Instance.Status == StageManager.EnumStageStatus.Playing;
        m_stageText.text = StageManager.Instance.StageName;
    }

    private void DispTimeText()
    {
        m_timeText.enabled = StageManager.Instance.Status == StageManager.EnumStageStatus.Playing;
        m_timeText.text = $"残り時間: {Mathf.CeilToInt(StageManager.Instance.TimeLimit)}";
    }

    private void MoveCamera()
    {
        if (StageManager.Instance.Status != StageManager.EnumStageStatus.Playing) return;
        m_camera.transform.position += new Vector3(StageManager.Instance.ScrollSpeed * Time.deltaTime, 0f, 0f);
    }
}
