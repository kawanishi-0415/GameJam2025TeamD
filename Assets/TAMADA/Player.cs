using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float normalJumpForce = 10f;
    public float highJumpForce = 18f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isCrouching;
    private float crouchStartTime;

    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

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
    }

    void Update()
    {
        // 地面チェック
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 左右移動
        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.F)) move = 1f;

        if (!isCrouching)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.J) && isGrounded)
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            isCrouching = true;
            crouchStartTime = Time.time;
            StartCrouch();
        }

        // しゃがみ解除
        if (Input.GetKeyUp(KeyCode.L))
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
