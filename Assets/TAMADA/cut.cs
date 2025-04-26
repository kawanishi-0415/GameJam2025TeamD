using UnityEngine;

public class CloneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // プレイヤーオブジェクトのプレハブ
    [SerializeField] private Transform spawnPoint;    // クローンの生成位置
    [SerializeField] private float jumpForce = 10f;   // ジャンプ力

    void Start()
    {
        // クローンを4つ生成
        CreateClone(KeyCode.A);
        CreateClone(KeyCode.F);
        CreateClone(KeyCode.J);
        CreateClone(KeyCode.L);
    }

    void CreateClone(KeyCode jumpKey)
    {
        // クローンを生成
        GameObject clone = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        PlayerCloneController playerController = clone.GetComponent<PlayerCloneController>();

        // クローンごとにジャンプキーを設定
        if (playerController != null)
        {
            playerController.SetJumpKey(jumpKey, jumpForce);
        }
    }
}

public class PlayerCloneController : MonoBehaviour
{
    [SerializeField] private KeyCode jumpKey;  // ジャンプするキー
    [SerializeField] private float jumpForce;  // ジャンプ力
    private Rigidbody2D rb;   // Rigidbody2Dコンポーネント

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ジャンプ処理
        if (Input.GetKeyDown(jumpKey) && Mathf.Abs(rb.velocity.y) < 0.1f)  // 地面にいる時のみジャンプ
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 移動処理を削除（これで本体は動かない）
    }

    // ジャンプキーとジャンプ力を設定するメソッド
    public void SetJumpKey(KeyCode key, float force)
    {
        jumpKey = key;
        jumpForce = force;
    }
}
