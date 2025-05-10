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

    public const float SET_CHANGE_TIME = 10f;

    private Camera m_camera = null;
    private PlayerController m_playerObj = null;
    private GameObject m_stageObj = null;
    private Coroutine m_playCoroutine = null;
    public string m_stageName { get; private set; } = "";
    public float ChangeTime { get; private set; } = SET_CHANGE_TIME;
    public float m_scrollSpeed { get; private set; } = 1f;


    [SerializeField] private Vector3 START_CAMERA_POS = new Vector3(0f, 0f, -10f);
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
        m_camera.transform.position = START_CAMERA_POS;
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
            StageManager.Instance.LoadSceneStatus = StageManager.EnumLoadSceneStatus.Init;
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

        // 初期値設定
        switch (StageManager.Instance.LoadSceneStatus)
        {
            case StageManager.EnumLoadSceneStatus.Init:
                StageManager.Instance.StageIndex = 0;
                StageManager.Instance.DeadNum = 0;
                StageManager.Instance.TimeLimit = StageManager.SET_TIMELIMIT;
                break;
            case StageManager.EnumLoadSceneStatus.Dead:
                break;
            case StageManager.EnumLoadSceneStatus.Clear:
                StageManager.Instance.TimeLimit = StageManager.SET_TIMELIMIT;
                break;
        }
        
        // ステージデータ取得
        SO_StageData stageData = StageManager.Instance.GetStageData();
        Vector2 startPosition = stageData.playerStartPosition;

        // Stage生成
        m_stageObj = Instantiate(stageData.stagePrefab);

        // プレイヤー位置設定
        switch (StageManager.Instance.LoadSceneStatus)
        {
            case StageManager.EnumLoadSceneStatus.Init:
                StageManager.Instance.PlayerPosition = stageData.playerStartPosition;
                break;
            case StageManager.EnumLoadSceneStatus.Dead:
                // "Save"レイヤーのLayerMaskを取得
                int saveLayer = LayerMask.NameToLayer("Save");
                List<GameObject> saveObjects = new List<GameObject>();
                FindAllSaveLayerObjects(m_stageObj.transform, saveLayer, saveObjects);
                foreach (var obj in saveObjects)
                {
                    if(StageManager.Instance.PlayerPosition.x >= obj.transform.position.x && startPosition.x <= obj.transform.position.x)
                    {
                        startPosition = obj.transform.position;
                        Vector3 cameraPos = START_CAMERA_POS;
                        cameraPos.x = obj.transform.position.x;
                        Camera.main.transform.position = cameraPos;
                    }
                }
                break;
            case StageManager.EnumLoadSceneStatus.Clear:
                StageManager.Instance.PlayerPosition = stageData.playerStartPosition;
                break;
        }

        // Player生成
        StageSceneController.Instance.CreatePlayer(startPosition);
        Scene currentScene = SceneManager.GetActiveScene();
        if (GameSceneManager.Instance.m_PrevSceneName == currentScene.name)
        {
            m_playerObj.SetKey(
                StageManager.Instance.LeftKeyCode,
                StageManager.Instance.RightKeyCode,
                StageManager.Instance.JumpKeyCode,
                StageManager.Instance.CrouchKeyCode);
        }
        ChangeOperationText();

        // FadeIn
        FadeManager.Instance.FadeIn();
        yield return new WaitUntil(() => FadeManager.Instance.Status == FadeManager.EnumStatus.End);

        Status = EnumStageStatus.Playing;
        m_stageName = stageData.stageName;
        m_scrollSpeed = stageData.scrollSpeed;
        yield return null;
    }

    private IEnumerator AsyncPlaying()
    {
        yield return null;

        StageManager.Instance.TimeLimit -= Time.deltaTime;
        if (StageManager.Instance.TimeLimit <= 0f)
        {
            StageManager.Instance.TimeLimit = 0f;
            SetTimeOver();
        }

        ChangeTime -= Time.deltaTime;
        if (ChangeTime <= 0f)
        {
            m_playerObj.ShuffleKeys();
            ChangeOperationText();
            ChangeTime = SET_CHANGE_TIME;
        }
    }

    private IEnumerator AsyncPlayerDead()
    {
        StageManager.Instance.DeadNum++;

        StageManager.Instance.PlayerPosition = m_playerObj.transform.position;

        ReloadCurrentScene(StageManager.EnumLoadSceneStatus.Dead);
        yield return null;
    }

    private IEnumerator AsyncTimeOver()
    {
        TimeOverWindow timeOverWindow = Instantiate(m_timeOverPrefab, m_canvas.transform);
        yield return new WaitUntil(() => !timeOverWindow.IsAnimation);

        // キー入力待機
        yield return new WaitUntil(() => Input.anyKeyDown);
        Status = EnumStageStatus.End;

        // FadeOut
        FadeManager.Instance.FadeOut();
        yield return new WaitUntil(() => FadeManager.Instance.Status == FadeManager.EnumStatus.End);

        GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.TITLE);
        yield return null;
    }

    private IEnumerator AsyncGameClear()
    {
        // SE鳴動
        StageManager.Instance.PlayGoleSe();

        GameClearWindow gameClearWindow = Instantiate(m_gameClearPrefab, m_canvas.transform);
        SO_StageData stageData = StageManager.Instance.GetStageData();
        gameClearWindow.SetText(stageData.stageName);
        yield return new WaitUntil(() => !gameClearWindow.IsAnimation);

        // キー入力待機
        yield return new WaitUntil(() => Input.anyKeyDown);
        Status = EnumStageStatus.End;

        StageManager.Instance.NextStage();

        if (StageManager.Instance.CheckAllClear())
        {
            GameSceneManager.Instance.m_DeadCount = StageManager.Instance.DeadNum;
            GameSceneManager.Instance.m_Time = StageManager.Instance.TimeLimit;
            GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.RESULT);
        }
        else
        {
            ReloadCurrentScene(StageManager.EnumLoadSceneStatus.Clear);
        }
        yield return null;
    }
    #endregion // Async

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
    public void ReloadCurrentScene(StageManager.EnumLoadSceneStatus loadSceneStatus)
    {
        StageManager.Instance.LoadSceneStatus = loadSceneStatus;
        GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.STAGE);
        Status = EnumStageStatus.End;
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
        StageManager.Instance.LeftKeyCode = m_playerObj.GetKeyByIndex(0);
        StageManager.Instance.RightKeyCode = m_playerObj.GetKeyByIndex(1);
        StageManager.Instance.JumpKeyCode = m_playerObj.GetKeyByIndex(2);
        StageManager.Instance.CrouchKeyCode = m_playerObj.GetKeyByIndex(3);
        m_operationControlelr.SetText(
            StageManager.Instance.LeftKeyCode,
            StageManager.Instance.RightKeyCode,
            StageManager.Instance.JumpKeyCode,
            StageManager.Instance.CrouchKeyCode);
    }

    private void DispStageText()
    {
        m_stageText.enabled = Status == EnumStageStatus.Playing;
        m_stageText.text = m_stageName;
    }

    private void DispTimeText()
    {
        m_timeText.enabled = Status == EnumStageStatus.Playing;
        m_timeText.text = $"残り時間: {Mathf.CeilToInt(StageManager.Instance.TimeLimit)}";
    }

    private void MoveCamera()
    {
        if (Status != EnumStageStatus.Playing) return;
        m_camera.transform.position += new Vector3(m_scrollSpeed * Time.deltaTime, 0f, 0f);
    }

    // 再帰的に階層を探索してSaveレイヤーのオブジェクトを取得
    void FindAllSaveLayerObjects(Transform parent, int saveLayer, List<GameObject> saveObjects)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.layer == saveLayer)
            {
                saveObjects.Add(child.gameObject);
            }

            // 子オブジェクトがある場合は再帰的に探索
            if (child.childCount > 0)
            {
                FindAllSaveLayerObjects(child, saveLayer, saveObjects);
            }
        }
    }
}
