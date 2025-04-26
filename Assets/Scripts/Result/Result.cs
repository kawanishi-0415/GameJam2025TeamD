using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour
{
    private GameSceneManager m_SceneManager = null;

    // Start is called before the first frame update
    void Start()
    {
        m_SceneManager = GameSceneManager.Instance;
        FadeManager.Instance.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        // ‰½‚©ƒNƒŠƒbƒN‚·‚é‚Æ‘JˆÚ‚·‚é
        if (Input.anyKey && FadeManager.Instance.Status == FadeManager.EnumStatus.End)
        {
            m_SceneManager.ChangeScene(GameSceneManager.GameState.TITLE);
        }
    }
}
