using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public int StageIndex { get; set; } = 0;
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
}
