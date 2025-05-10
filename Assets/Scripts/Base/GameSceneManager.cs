using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{

    public static GameSceneManager Instance { get; private set; }
    public enum GameState
    {
        TITLE,
        STAGE,
        RESULT,
    }
    private GameState m_State = GameState.TITLE;
    private string[] m_SceneName = { "Title", "Stage", "Result" };
    private bool m_IsFadeing = false;

    public string m_PrevSceneName = "";
    public float m_Time = 0.0f;
    public int m_DeadCount = 0;
   

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

        DontDestroyOnLoad(Instance);
    }

    // シーン遷移(同期)
    private void LoadScene(string sceneName)
    {
        m_PrevSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    // シーン遷移(フェードアウト終了後に遷移)
    public void ChangeScene(GameState state)
    {
        if (m_IsFadeing)
        {
            return;
        }
        m_IsFadeing = true;
        FadeManager.Instance.FadeOut();
        StartCoroutine(WaitFade(state));
    }

    // フェード待機
    private IEnumerator WaitFade(GameState state)
    {
        // フェード待機
        while (FadeManager.Instance.Status == FadeManager.EnumStatus.Fading)
        {
            yield return null;
        }

        m_State = state;
        LoadScene(m_SceneName[(int)state]);
        m_IsFadeing = false;
    }

}
