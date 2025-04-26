using UnityEngine;

public class Player_Cuted : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;   // 進む速さ
    [SerializeField] private float jumpForce = 10f;  // ジャンプの強さ
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // ジャンプするキーをインスペクターで選択

    private Rigidbody2D rb;  // Rigidbody2Dコンポーネント

    void Start()
    {
        // Rigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // キャラクターを右に進める
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // ジャンプ処理
        if (Input.GetKeyDown(jumpKey) && Mathf.Abs(rb.velocity.y) < 0.01f)  // 地面にいる時のみジャンプ
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
