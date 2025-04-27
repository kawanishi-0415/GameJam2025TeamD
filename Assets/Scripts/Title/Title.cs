using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField]
    private Text m_ScenarioText = null;
    private GameSceneManager m_SceneManager = null;

    private List<string> m_TitleTextBlockList = new List<string>();
    private int m_TextBlockIndex = 0;
    private bool m_IsTextEnd = false;
    private bool m_IsTextFadeEnd = true;
    private bool m_IsWaitText = false;
    [SerializeField]
    private float m_TextStartWaitTime = 1.0f;
    [SerializeField]
    private float m_TextFadeTime = 1.0f;
    [SerializeField]
    private Color m_TextColor = new Color(0.0f,0.0f,0.0f,1.0f);

    // Start is called before the first frame update
    void Start()
    {
        m_SceneManager = GameSceneManager.Instance;
        FadeManager.Instance.FadeIn();
        m_ScenarioText.enabled = false;

        if (LoadTextFile())
        {
            StartCoroutine(WaitFirstText());
        }
        else
        {
            m_IsTextEnd = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsTextEnd)
        {
            if (!m_IsWaitText) 
            {
                if (m_IsTextFadeEnd)
                {
                    if (m_ScenarioText.color.a <= 0.0f)
                    {
                        // 次のテキスト
                        if (NextTextBlock())
                        {
                            StartCoroutine(TextFadeIn());
                        }
                    }
                    else
                    {
                        if (Input.anyKey)
                        {
                            StartCoroutine(TextFadeOut());
                        }
                    }

                }
            }
        }
        else
        {
            // 何かクリックすると遷移する
            if (Input.anyKey && FadeManager.Instance.Status == FadeManager.EnumStatus.End)
            {
                m_SceneManager.ChangeScene(GameSceneManager.GameState.STAGE);
            }
        }
        
    }

    private bool LoadTextFile()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Scenario/title");
        
        if (textAsset != null)
        {
            // テキストファイルの内容を取得
            string text = textAsset.text;
            // テキストをブロックごとに分ける
            string[] lines = text.Split("\r\n");
            string str = "";
            foreach (string line in lines)
            {
                if (line != "")
                {
                    if (str != "")
                    {
                        str += "\n";
                    }
                    str += line;
                }
                else
                {
                    m_TitleTextBlockList.Add(str);
                    str = "";
                }
            }

            return 0 < m_TitleTextBlockList.Count;
        }

        return false;
    }

    private bool ShowScenarioText()
    {
        if (0 < m_TitleTextBlockList.Count)
        {
            m_TextBlockIndex = 0;
            m_ScenarioText.text = m_TitleTextBlockList[m_TextBlockIndex];

            m_ScenarioText.enabled = true;
            return true;
        }

        return false;
    }

    private IEnumerator TextFadeIn()
    {
        if (m_IsTextFadeEnd)
        {
            m_IsTextFadeEnd = false;
            float time = m_TextFadeTime;
            Color textColor = m_TextColor;
            textColor.a = 0.0f;

            while (0 <= time)
            {
                textColor.a = 1.0f - (time / m_TextFadeTime); 
                m_ScenarioText.color = textColor;

                time -= Time.deltaTime;
                yield return null;
            }
            textColor.a = 1.0f;
            m_ScenarioText.color = textColor;
            m_IsTextFadeEnd = true;
        }
        

        yield return null;  
    }

    private IEnumerator TextFadeOut()
    {
        if (m_IsTextFadeEnd)
        {
            m_IsTextFadeEnd = false;
            float time = m_TextFadeTime;
            Color textColor = m_TextColor;
            textColor.a = 1.0f;

            while (0 <= time)
            {
                textColor.a = time / m_TextFadeTime;
                m_ScenarioText.color = textColor;

                time -= Time.deltaTime;
                yield return null;
            }
            textColor.a = 0.0f;
            m_ScenarioText.color = textColor;
            m_IsTextFadeEnd = true;

        }
            

        yield return null;
    }

    private IEnumerator WaitFirstText()
    {
        m_IsWaitText = true;
        float time = m_TextStartWaitTime;
        while (0 <= time)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if (ShowScenarioText())
        {
            StartCoroutine(TextFadeIn());
        }
        m_IsWaitText = false;
        yield return null;
    }

    private bool NextTextBlock()
    {
        if (m_TitleTextBlockList.Count <= m_TextBlockIndex + 1)
        {
            m_IsTextEnd = true;
            return false;
        }

        m_TextBlockIndex++;
        m_ScenarioText.text = m_TitleTextBlockList[m_TextBlockIndex];
        return true;
    }
}
