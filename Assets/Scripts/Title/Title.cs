using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{

    private GameSceneManager m_SceneManager = null;



    // Start is called before the first frame update
    void Start()
    {
        m_SceneManager = FindObjectOfType<GameSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // ‰½‚©ƒNƒŠƒbƒN‚·‚é‚Æ‘JˆÚ‚·‚é
        if (Input.anyKey)
        {
            m_SceneManager.ChangeScene(GameSceneManager.GameState.GAME);
        }
    }
}
