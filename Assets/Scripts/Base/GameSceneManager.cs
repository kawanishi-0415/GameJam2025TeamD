using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    
    private static GameSceneManager m_Instance = null;
    public enum GameState
    {
        TITLE,
        GAME,
        RESULT,
    }
    private GameState m_State = GameState.TITLE;
    private string[] m_SceneName = { "Title", "Stage", "Game"};

    public static GameSceneManager GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameSceneManager();
        }

        return m_Instance;
    }

    // ƒV[ƒ“‘JˆÚ(“¯Šú)
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(GameState state)
    {
        m_State = state;
        LoadScene(m_SceneName[(int)state]);
    }

}
