using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager m_SceneManager = null;


    // Start is called before the first frame update
    void Start()
    {
        m_SceneManager.ChangeScene(GameSceneManager.GameState.GAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
