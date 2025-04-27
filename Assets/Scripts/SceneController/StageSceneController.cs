using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneController : MonoBehaviour
{
    public static StageSceneController Instance { get; private set; }

    public enum EnumStageStatus
    {
        Init,
        Playing,
        PlayerDead,
        TimeOver,
        GameClear,
        End,
    }
    public EnumStageStatus Status { get; set; } = EnumStageStatus.Init;

    public const float SET_CHANGE_TIME = 20f;
    public const float SET_TIMELIMIT = 300f;

    private Camera m_camera = null;
    private PlayerController m_playerObj = null;
    private GameObject m_stageObj = null;
    private Coroutine m_playCoroutine = null;
    public string m_stageName { get; private set; } = "";
    public int m_deadNum = 0;
    public float m_timeLimit { get; private set; } = SET_TIMELIMIT;
    public float ChangeTime { get; private set; } = SET_CHANGE_TIME;
    public float m_scrollSpeed { get; private set; } = 1f;


    [SerializeField] private Vector3 m_startCameraPos = new Vector3(0f, 0f, -10f);
    [SerializeField] private TextMeshProUGUI m_stageText = null;
    [SerializeField] private TextMeshProUGUI m_timeText = null;
    [SerializeField] private OperationController m_operationControlelr = null;
    [SerializeField] private PlayerController m_playerPrefab = null;
    [SerializeField] private Canvas m_canvas = null;
    [SerializeField] private TimeOverWindow m_timeOverPrefab = null;
    [SerializeField] private GameClearWindow m_gameClearPrefab = null;
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
        PlayStart();
    }

    private void Update()
    {
        // スクロール
        MoveCamera();

        // UI表示
        DispStageText();
        DispTimeText();
    }

    public void PlayStart()
    {
        if (m_playCoroutine != null)
            StopCoroutine(m_playCoroutine);

        Status = EnumStageStatus.Init;
        ChangeTime = SET_CHANGE_TIME;

        Scene currentScene = SceneManager.GetActiveScene();
        if (GameSceneManager.Instance.m_PrevSceneName != currentScene.name)
        {
            Initialized();
        }

        m_playCoroutine = StartCoroutine(CoLoop());
    }

    private IEnumerator CoLoop()
    {
        while (Status != EnumStageStatus.End)
        {
            switch (Status)
            {
                case EnumStageStatus.Init:
                    yield return AsyncInit();
                    break;
                case EnumStageStatus.Playing:
                    yield return AsyncPlaying();
                    break;
                case EnumStageStatus.PlayerDead:
                    yield return AsyncPlayerDead();
                    break;
                case EnumStageStatus.TimeOver:
                    yield return AsyncTimeOver();
                    break;
                case EnumStageStatus.GameClear:
                    yield return AsyncGameClear();
                    break;
            }
        }
    }

    #region Async
    private IEnumerator AsyncInit()
    {
        if (m_stageObj != null)
            Destroy(m_stageObj);

        // Stage生成
        SO_StageData stageData = StageManager.Instance.GetStageData();
        m_stageObj = Instantiate(stageData.stagePrefab);

        // FadeIn
        FadeManager.Instance.FadeIn();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        // Player生成
        StageSceneController.Instance.CreatePlayer(stageData.playerStartPosition);
        ChangeOperationText();
        yield return new WaitForSeconds(1.0f);

        Status = EnumStageStatus.Playing;
        m_stageName = stageData.stageName;
        m_scrollSpeed = stageData.scrollSpeed;
        yield return null;
    }

    private IEnumerator AsyncPlaying()
    {
        yield return null;

        m_timeLimit -= Time.deltaTime;
        if (m_timeLimit <= 0f)
        {
            m_timeLimit = 0f;
            SetTimeOver();
        }

        ChangeTime -= Time.deltaTime;
        if (ChangeTime <= 0f)
        {
            m_playerObj.ShuffleKeys();
            ChangeTime = SET_CHANGE_TIME;
        }
    }

    private IEnumerator AsyncPlayerDead()
    {
        m_deadNum++;

        // FadeOut
        FadeManager.Instance.FadeOut();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        ReloadCurrentScene();
        yield return null;
    }

    private IEnumerator AsyncTimeOver()
    {
        TimeOverWindow timeOverWindow = Instantiate(m_timeOverPrefab, m_canvas.transform);
        yield return new WaitUntil(() => !timeOverWindow.IsAnimation);

        // キー入力待機
        yield return new WaitUntil(() => Input.anyKeyDown);

        // FadeOut
        FadeManager.Instance.FadeOut();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.TITLE);
        yield return null;
    }

    private IEnumerator AsyncGameClear()
    {
        GameClearWindow gameClearWindow = Instantiate(m_gameClearPrefab, m_canvas.transform);
        SO_StageData stageData = StageManager.Instance.GetStageData();
        gameClearWindow.SetText(stageData.stageName);
        yield return new WaitUntil(() => !gameClearWindow.IsAnimation);

        // キー入力待機
        yield return new WaitUntil(() => Input.anyKeyDown);

        StageManager.Instance.NextStage();

        // FadeOut
        FadeManager.Instance.FadeOut();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        if (StageManager.Instance.CheckAllClear())
        {
            GameSceneManager.Instance.m_DeadCount = m_deadNum;
            GameSceneManager.Instance.m_Time = m_timeLimit;
            GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.RESULT);
        }
        else
        {
            ReloadCurrentScene();
        }
        yield return null;
    }
    #endregion // Async

    public void Initialized()
    {
        StageManager.Instance.StageIndex = 0;
        m_timeLimit = SET_TIMELIMIT;
    }

    /// <summary>
    /// Player死亡
    /// </summary>
    public void SetPlayerDead()
    {
        Status = EnumStageStatus.PlayerDead;
    }

    /// <summary>
    /// タイムオーバー
    /// </summary>
    public void SetTimeOver()
    {
        Status = EnumStageStatus.TimeOver;
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    public void SetGameClear()
    {
        Status = EnumStageStatus.GameClear;
    }

    /// <summary>
    /// シーンを再度ロードする
    /// </summary>
    public void ReloadCurrentScene()
    {
        GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.STAGE);
    }


    public void CreatePlayer(Vector2 startPosition)
    {
        if (m_playerObj != null)
            Destroy(m_playerObj.gameObject);

        m_playerObj = Instantiate(StageSceneController.Instance.PlayerPrefab);
        m_playerObj.transform.position = startPosition;
    }

    /// <summary>
    /// 操作方法変更
    /// </summary>
    public void ChangeOperation()
    {
        m_playerObj.ShuffleKeys();
        ChangeOperationText();
    }

    private void ChangeOperationText()
    {
        KeyCode leftCode = m_playerObj.GetKeyByIndex(0);
        KeyCode rightCode = m_playerObj.GetKeyByIndex(1);
        KeyCode jumpCode = m_playerObj.GetKeyByIndex(2);
        KeyCode crouchCode = m_playerObj.GetKeyByIndex(3);
        m_operationControlelr.SetText(leftCode.ToString(), rightCode.ToString(), jumpCode.ToString(), crouchCode.ToString());
    }

    private void DispStageText()
    {
        m_stageText.enabled = Status == EnumStageStatus.Playing;
        m_stageText.text = m_stageName;
    }

    private void DispTimeText()
    {
        m_timeText.enabled = Status == EnumStageStatus.Playing;
        m_timeText.text = $"残り時間: {Mathf.CeilToInt(m_timeLimit)}";
    }

    private void MoveCamera()
    {
        if (Status != EnumStageStatus.Playing) return;
        m_camera.transform.position += new Vector3(m_scrollSpeed * Time.deltaTime, 0f, 0f);
    }
}
