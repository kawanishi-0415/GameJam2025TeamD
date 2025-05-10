using UnityEngine;

public class sippo : MonoBehaviour
{
    private TrailRenderer trail;

    void Start()
    {
        // Trail Renderer がアタッチされているか確認、なければ追加
        trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }

        // ★ トレイルの見た目設定 ★

        // trail の長さを一定に保つため、表示時間を短めに固定
        trail.time = 1.4f; // 速度に応じて調整（例：0.3秒間だけ表示）

        // 太さ一定
        AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        trail.widthCurve = curve;

        // 全体の太さ倍率
        trail.widthMultiplier = 0.3f;

        // 色（白→白、常に不透明）
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            }
        );
        trail.colorGradient = gradient;

        // 材質（必須）
        trail.material = new Material(Shader.Find("Sprites/Default"));

        // デフォルトは非表示にしておく
        trail.emitting = false;
    }

    void Update()
    {
        // 動いているときだけ trail を表示
        float move = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.R) ? 1f : 0f;
        trail.emitting = move != 0;
    }
}
