using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public enum EnumStageStatus
    {
        Init,
        Playing,
        GameOver,
        GameClear,
        End,
    }
    private EnumStageStatus m_status = EnumStageStatus.Init;

    public int StageNum { get; set; } = 0;
    private GameObject m_stageObj = null;
    private PlayerController m_playerObj = null;
    public float TimeLimit { get; private set; } = 0f;

    [SerializeField] private SO_StageDatabase m_so_StageDataBase = null;
    [SerializeField] private PlayerController m_playerPrefab = null;

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

    /// <summary>
    /// 次ステージへ変更
    /// </summary>
    public void NextStage()
    {
        StageNum++;
        if (StageNum >= m_so_StageDataBase.stageList.Count)
        {
            StageNum = 0;
        }
    }

    private Coroutine m_playCoroutine = null; 
    public void PlayStart()
    {
        if(m_playCoroutine != null)
            StopCoroutine(m_playCoroutine);

        m_status = EnumStageStatus.Init;
        m_playCoroutine = StartCoroutine(CoLoop());
    }

    private IEnumerator CoLoop()
    {
        while(m_status != EnumStageStatus.End)
        {
            switch (m_status)
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

    private void DestroyObj()
    {
        if (m_stageObj != null)
            Destroy(m_stageObj);
        if (m_playerObj != null)
            Destroy(m_playerObj.gameObject);
    }

    #region Async
    private IEnumerator AsyncInit()
    {
        DestroyObj();

        // Stage生成
        SO_StageData stageData = GetStageData();
        m_stageObj = Instantiate(stageData.stagePrefab);

        // FadeIn
        FadeManager.Instance.FadeIn();
        yield return FadeManager.Instance.Status == FadeManager.EnumStatus.End;

        // Player生成
        m_playerObj = Instantiate(m_playerPrefab);
        m_playerObj.transform.position = stageData.playerStartPosition;
        yield return new WaitForSeconds(1.0f);

        m_status = EnumStageStatus.Playing;
        TimeLimit = stageData.timeLimit;
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
            GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
            gameSceneManager.ChangeScene(GameSceneManager.GameState.RESULT);
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
        if (StageNum >= m_so_StageDataBase.stageList.Count)
        {
            Debug.LogError($"ターゲット外のstageNumが定義されています。m_stageNum = {StageNum}");
        }

        return m_so_StageDataBase.stageList[StageNum];
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public void SetGameOver()
    {
        m_status = EnumStageStatus.GameOver;
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    public void SetGameClear()
    {
        m_status = EnumStageStatus.GameClear;
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
        return StageNum == 0;
    }
}
