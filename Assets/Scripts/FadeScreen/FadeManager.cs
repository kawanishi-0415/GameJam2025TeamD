using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public enum EnumStatus
    {
        Fading,
        End,
    }
    public EnumStatus Status { get; private set; } = EnumStatus.End;

    public static FadeManager Instance { get; private set; }

    [SerializeField] private CanvasGroup m_canvasGroup = null;
    [SerializeField] private Image m_fadeScreen = null;

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

    public void FadeIn(Color color, float time)
    {
        m_fadeScreen.color = color;
        Fade(time, 1f, 0f);
    }
    public void FadeIn(float time)
    {
        Fade(time, 1f, 0f);
    }
    public void FadeIn()
    {
        Fade(1f, 1f, 0f);
    }
    public void FadeOut(Color color, float time)
    {
        m_fadeScreen.color = color;
        Fade(time, 0f, 1f);
    }
    public void FadeOut(float time)
    {
        Fade(time, 0f, 1f);
    }
    public void FadeOut()
    {
        Fade(1f, 0f, 1f);
    }

    public void Fade(float time, float start, float end)
    {
        m_canvasGroup.alpha = start;
        m_canvasGroup.blocksRaycasts = true;

        Status = EnumStatus.Fading;
        m_canvasGroup.DOFade(end, time).OnComplete(() => {
            Status = EnumStatus.End;
            m_canvasGroup.blocksRaycasts = false;
        });
    }
}
