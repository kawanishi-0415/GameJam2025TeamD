using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TimeOverWindow : MonoBehaviour
{
    [SerializeField] private RectTransform m_targetWindow;
    [SerializeField] private float m_time = 2.0f;
    public bool IsAnimation { get; private set; } = true;

    private void Start()
    {
        // 画面サイズを取得
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // 最初に画面の上（外）に配置
        m_targetWindow.anchoredPosition = new Vector2(0, screenSize.y / 2 + m_targetWindow.rect.height / 2);

        // DOTweenで中央まで移動
        m_targetWindow.DOAnchorPos(Vector2.zero, m_time).SetEase(Ease.OutBounce).OnComplete(() => {
            IsAnimation = false;
        });
    }
}
