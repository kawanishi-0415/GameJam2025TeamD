using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public enum EnumLoadSceneStatus
    {
        Init,
        Dead,
        Clear,
    }
    public EnumLoadSceneStatus LoadSceneStatus { get; set; } = EnumLoadSceneStatus.Init;
    public Vector2 PlayerPosition { get; set; } = Vector2.zero;

    public const float SET_TIMELIMIT = 60f;
    public float TimeLimit { get; set; } = SET_TIMELIMIT;

    [SerializeField] private int m_deadNum = 0;
    public int DeadNum { get { return m_deadNum; } set { m_deadNum = value; } }

    [SerializeField] private int m_stageIndex = 0;
    public int StageIndex {  get { return m_stageIndex; } set { m_stageIndex = value; } }
    [SerializeField] private SO_StageDatabase m_so_StageDataBase = null;
    [SerializeField] private AudioSource m_audioGole1 = null;
    [SerializeField] private AudioSource m_audioGole2 = null;

    public KeyCode LeftKeyCode { get; set; } = KeyCode.Q;
    public KeyCode RightKeyCode { get; set; } = KeyCode.R;
    public KeyCode JumpKeyCode { get; set; } = KeyCode.U;
    public KeyCode CrouchKeyCode { get; set; } = KeyCode.P;

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
        StageIndex++;
        if (StageIndex >= m_so_StageDataBase.stageList.Count)
        {
            StageIndex = 0;
        }
    }

    /// <summary>
    /// ステージデータ取得
    /// </summary>
    /// <returns></returns>
    public SO_StageData GetStageData()
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
        StageSceneController.Instance.Status = StageSceneController.EnumStageStatus.PlayerDead;
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    public void SetGameClear()
    {
        StageSceneController.Instance.Status = StageSceneController.EnumStageStatus.GameClear;
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

    public void PlayGoleSe()
    {
        if (StageIndex == m_so_StageDataBase.stageList.Count - 1)
        {
            m_audioGole1.Play();
        }

        m_audioGole2.Play();
    }
}
