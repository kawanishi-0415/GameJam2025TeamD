using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public const float SET_TIMELIMIT = 300f;

    public enum EnumStageStatus
    {
        Init,
        Playing,
        GameOver,
        GameClear,
        End,
    }
    public EnumStageStatus Status { get; private set; } = EnumStageStatus.Init;

    public int StageIndex { get; set; } = 0;
    private GameObject m_stageObj = null;
    public string StageName { get; private set; } = "";
    public float TimeLimit { get; private set; } = SET_TIMELIMIT;
    public float ScrollSpeed { get; private set; } = 1f;
    private Coroutine m_playCoroutine = null;

    [SerializeField] private SO_StageDatabase m_so_StageDataBase = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されない
        }
        else
        {
            Destroy(gameObject); // 2個目以降は自動で破棄する
        }
    }

    public void Initialized()
    {
        StageIndex = 0;
        TimeLimit = SET_TIMELIMIT;
    }

    /// <summary>
    /// 次ステージへ変更
    /// </summary>
    public void NextStage()
    {
        StageIndex++;
        if (StageIndex >= m_so_StageDataBase.stageList.Count)
        {
            StageIndex = 0;
        }
    }

    public void PlayStart()
    {
        if(m_playCoroutine != null)
            StopCoroutine(m_playCoroutine);

        Status = EnumStageStatus.Init;
        m_playCoroutine = StartCoroutine(CoLoop());
    }

    private IEnumerator CoLoop()
    {
        while(Status != EnumStageStatus.End)
        {
            switch (Status)
            {
                case EnumStageStatus.Init:
                    yield return AsyncInit();
                    break;
                case EnumStageStatus.Playing:
                    yield return AsyncPlaying();
                    break;
                case EnumStageStatus.GameOver:
                    yield return AsyncGameOver();
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
        SO_StageData stageData = GetStageData();
        m_stageObj = Instantiate(stageData.stagePrefab);

        // FadeIn
        FadeManager.Instance.FadeIn();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        // Player生成
        StageSceneController.Instance.CreatePlayer(stageData.playerStartPosition);
        yield return new WaitForSeconds(1.0f);

        Status = EnumStageStatus.Playing;
        StageName = stageData.stageName;
        ScrollSpeed = stageData.scrollSpeed;
        yield return null;
    }

    private IEnumerator AsyncPlaying()
    {
        yield return null;

        TimeLimit -= Time.deltaTime;
        if(TimeLimit <= 0f)
        {
            SetGameOver();
        }
    }

    private IEnumerator AsyncGameOver()
    {
        // FadeOut
        FadeManager.Instance.FadeOut();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        ReloadCurrentScene();
        yield return null;
    }

    private IEnumerator AsyncGameClear()
    {
        NextStage();

        // FadeOut
        FadeManager.Instance.FadeOut();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        if (CheckAllClear())
        {
            Initialized();
            GameSceneManager.Instance.ChangeScene(GameSceneManager.GameState.RESULT);
        }
        else
        {
            ReloadCurrentScene();
        }
        yield return null;
    }
    #endregion // Async

    /// <summary>
    /// ステージデータ取得
    /// </summary>
    /// <returns></returns>
    private SO_StageData GetStageData()
    {
        if (StageIndex >= m_so_StageDataBase.stageList.Count)
        {
            Debug.LogError($"ターゲット外のstageNumが定義されています。m_stageNum = {StageIndex}");
        }

        return m_so_StageDataBase.stageList[StageIndex];
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public void SetGameOver()
    {
        Status = EnumStageStatus.GameOver;
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
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// 全てのステージをクリアしたかのチェック
    /// NextStage()でステージ外まで行った場合は0としているので0の場合は全てクリアとする
    /// </summary>
    /// <returns></returns>
    public bool CheckAllClear()
    {
        return StageIndex == 0;
    }
}
