using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_StateObj = new List<GameObject>();
    [SerializeField]
    private Image m_Image;
    [SerializeField]
    private List<Sprite> m_TextureList = new List<Sprite>();

    public enum ResultState
    {
        GAME_RESULT,
        SCNARIO_RESULT,
        END_RESULT,
    }

    private ResultState m_State = ResultState.GAME_RESULT;
    private int m_TextureIndex = 0;


    private GameSceneManager m_SceneManager = null;

    // Start is called before the first frame update
    void Start()
    {
        m_SceneManager = GameSceneManager.Instance;
        FadeManager.Instance.FadeIn();

        m_State = ResultState.GAME_RESULT;
        EnableState();

        
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case ResultState.GAME_RESULT:
                // ゲームリザルト
                if (Input.anyKey)
                {
                    m_State = ResultState.SCNARIO_RESULT;
                }
                break;

            case ResultState.SCNARIO_RESULT:
                // シナリオ

                if (Input.anyKey)
                {
                    if (m_TextureList.Count <= m_TextureIndex)
                    {
                        m_Image.enabled = false;
                        m_State = ResultState.SCNARIO_RESULT;
                    }
                    else
                    {
                        m_TextureIndex++;
                        m_Image.sprite = m_TextureList[m_TextureIndex];
                    }
                }

                break;
            case ResultState.END_RESULT:
                // 何かクリックすると遷移する
                if (Input.anyKey && FadeManager.Instance.Status == FadeManager.EnumStatus.End)
                {
                    m_SceneManager.ChangeScene(GameSceneManager.GameState.TITLE);
                }
                break;

            default:
                break;
        }        
    }

    private void EnableState()
    {
        switch (m_State)
        {
            case ResultState.GAME_RESULT:
                m_StateObj[0].SetActive(true);
                m_StateObj[1].SetActive(false);
                break;
            case ResultState.SCNARIO_RESULT:
                m_StateObj[0].SetActive(false);
                m_StateObj[1].SetActive(true);
                break;
            case ResultState.END_RESULT:
                m_StateObj[0].SetActive(false);
                m_StateObj[1].SetActive(false);
                break;
            default:
                break;
        }
    }
}
