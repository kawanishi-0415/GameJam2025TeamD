using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float normalJumpForce = 10f;
    [SerializeField] private float highJumpForce = 18f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isCrouching;
    private float crouchStartTime;

    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    // 設定した操作ボタン
    private KeyCode moveLeftKey = KeyCode.A;
    private KeyCode moveRightKey = KeyCode.F;
    private KeyCode jumpKey = KeyCode.J;
    private KeyCode crouchKey = KeyCode.L;

    // 操作ボタンのリスト
    private List<KeyCode> actionKeys;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        originalScale = transform.localScale;

        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }

        // 初期設定の操作ボタンをリストに格納
        actionKeys = new List<KeyCode> { moveLeftKey, moveRightKey, jumpKey, crouchKey };
    }

    void Update()
    {
        // 地面チェック
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 左右移動
        float move = 0f;
        if (Input.GetKey(moveLeftKey)) move = -1f;
        if (Input.GetKey(moveRightKey)) move = 1f;

        if (!isCrouching)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        // ジャンプ処理
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            float jumpForce = normalJumpForce;

            if (isCrouching)
            {
                float crouchDuration = Time.time - crouchStartTime;
                if (crouchDuration >= 1f)
                {
                    jumpForce = highJumpForce;
                }
                isCrouching = false;
                ResetCrouch(); // 縮小戻す
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // しゃがみ開始
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            crouchStartTime = Time.time;
            StartCrouch();
        }

        // しゃがみ解除
        if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
            ResetCrouch();
        }
    }

    void StartCrouch()
    {
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
        }
    }

    void ResetCrouch()
    {
        transform.localScale = originalScale;

        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    // 操作ボタンをランダムに入れ替えるメソッド
    public void ChangeControlButtons()
    {
        Shuffle(actionKeys);

        moveLeftKey = actionKeys[0];
        moveRightKey = actionKeys[1];
        jumpKey = actionKeys[2];
        crouchKey = actionKeys[3];

        Debug.Log($"New Controls: Move Left - {moveLeftKey}, Move Right - {moveRightKey}, Jump - {jumpKey}, Crouch - {crouchKey}");
    }

    // リストをシャッフルするメソッド
    private void Shuffle(List<KeyCode> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            KeyCode temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
