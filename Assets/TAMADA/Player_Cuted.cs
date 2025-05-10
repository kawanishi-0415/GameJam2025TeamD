using UnityEngine;

public class Player_Cuted : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;   // 進む速さ
    [SerializeField] private float jumpForce = 10f;  // ジャンプの強さ
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // ジャンプするキーをインスペクターで選択
    [SerializeField] private GameObject objectToShow;  // 表示するオブジェクト
    [SerializeField] private Vector3 spawnPosition;  // 表示オブジェクトのスポーン位置

    private Rigidbody2D rb;  // Rigidbody2Dコンポーネント
    private Camera mainCamera; // メインカメラ

    void Start()
    {
        // Rigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        // メインカメラを取得
        mainCamera = Camera.main;
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

        // カメラの外に出ないようにする（下方向だけ自由）
        ClampPositionToCameraSidesAndTop();

     //   // LキーでLinkメソッド呼び出し
     //  if (Input.GetKeyDown(KeyCode.O))
     // {
     //       Link();
     //}
  }

    private void ClampPositionToCameraSidesAndTop()
    {
        if (mainCamera == null) return;

        Vector3 pos = transform.position;

        // カメラの左下と右上位置を取得
        Vector3 min = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 max = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // 左右だけClampする
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);

        // 上方向だけClampする
        if (pos.y > max.y)
        {
            pos.y = max.y;
        }

        // 下には出てもいいので、yが小さくなるのは自由！

        transform.position = pos;

        // 画面外に出た場合、ゲームオーバー処理
        if (pos.y < min.y)  // 画面の下に出たら
        {
            StageManager.Instance.SetGameOver();
        }
    }

    public void SetPlayerObjShow(GameObject obj)
    {
        objectToShow = obj;
    }

    // Linkメソッドで指定したオブジェクトを表示し、その後このオブジェクトを非表示にする
    public void Link()
    {
        if (objectToShow != null)
        {
            // objectToShowの位置を指定したオブジェクトの位置に設定
            objectToShow.transform.position = transform.position;  // ここで現在のプレイヤーの位置に設定
            objectToShow.SetActive(true);  // オブジェクトを表示
        }

        // このオブジェクトを非表示にする
        gameObject.SetActive(false);
    }
}
