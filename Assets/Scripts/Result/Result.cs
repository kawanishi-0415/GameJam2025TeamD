using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Result : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_StateObj = new List<GameObject>();
    [SerializeField]
    private Image m_Image;
    [SerializeField]
    private List<Sprite> m_TextureList = new List<Sprite>();

    [SerializeField]
    private TextMeshProUGUI m_TimeText;
    [SerializeField]
    private TextMeshProUGUI m_DeadCountText;
    [SerializeField]
    private float m_WaitTime = 0.5f;

    private float m_WaitTimeCnt = 0.0f;

    [SerializeField]
    private float m_FadeTime = 0.5f;
    private float m_FadeCnt = 0.0f;

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
                if (Input.anyKey && FadeManager.Instance.Status == FadeManager.EnumStatus.End)
                {
                    m_State = ResultState.SCNARIO_RESULT;
                    EnableState();
                }
                break;

            case ResultState.SCNARIO_RESULT:
                // シナリオ

                if (m_WaitTimeCnt <= 0.0f)
                {
                    if (Input.anyKey)
                    {
                        if (m_TextureList.Count <= m_TextureIndex + 1)
                        {
                            //m_Image.enabled = false;
                            m_State = ResultState.END_RESULT;
                            EnableState();
                        }
                        else
                        {
                            m_TextureIndex++;
                            m_Image.sprite = m_TextureList[m_TextureIndex];
                        }

                        m_WaitTimeCnt = m_WaitTime;
                    }
                }
                else
                {
                    m_WaitTimeCnt -= Time.deltaTime;
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
                m_TimeText.text = "残り時間 " + m_SceneManager.m_Time.ToString("F0") + " 秒";
                m_DeadCountText.text = "死んだ回数 " + m_SceneManager.m_DeadCount.ToString() + " 回";
                break;
            case ResultState.SCNARIO_RESULT:
                m_StateObj[0].SetActive(false);
                m_StateObj[1].SetActive(true);
                m_Image.sprite = m_TextureList[m_TextureIndex];
                m_WaitTimeCnt = m_WaitTime;
                break;
            case ResultState.END_RESULT:
                m_StateObj[0].SetActive(false);
                m_StateObj[1].SetActive(true);
                break;
            default:
                break;
        }
    }

    private IEnumerator ScenarioFade()
    {
        while (0.0f < m_FadeCnt)
        {
            m_FadeCnt -= Time.deltaTime;
            yield return null;
        }
    }
}
