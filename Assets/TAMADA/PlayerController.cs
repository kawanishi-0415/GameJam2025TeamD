using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("移動とジャンプ設定")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float normalJumpForce = 10f;
    [SerializeField] private float highJumpForce = 18f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("非表示にしておくオブジェクト4つ")]
    [SerializeField] private GameObject[] hiddenObjects = new GameObject[4];

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isCrouching;
    private float crouchStartTime;
    private bool isGameOver = false;

    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private KeyCode moveLeftKey = KeyCode.A;
    private KeyCode moveRightKey = KeyCode.F;
    private KeyCode jumpKey = KeyCode.J;
    private KeyCode crouchKey = KeyCode.L;

    private List<KeyCode> actionKeys;

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        mainCamera = Camera.main;

        originalScale = transform.localScale;

        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }

        // 最初はすべて非表示
        SetObjectsActive(false);

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
                ResetCrouch();
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

        ClampPositionAndCheckFall();
    }

    private void StartCrouch()
    {
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
        }
    }

    private void ResetCrouch()
    {
        transform.localScale = originalScale;

        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    private void ClampPositionAndCheckFall()
    {
        Vector3 pos = transform.position;

        Vector3 min = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 max = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float halfWidth = boxCollider.bounds.extents.x;
        float halfHeight = boxCollider.bounds.extents.y;

        pos.x = Mathf.Clamp(pos.x, min.x + halfWidth, max.x - halfWidth);
        pos.y = Mathf.Clamp(pos.y, min.y - 100f, max.y - halfHeight);

        transform.position = pos;

        if (!isGameOver && transform.position.y < min.y - 1f)
        {
            isGameOver = true;
            if (StageManager.Instance != null)
            {
                StageManager.Instance.SetGameOver();
            }
            else
            {
                Debug.LogWarning("StageManager.Instance が存在しません！");
            }
        }
    }

    // Cutメソッド（非表示のオブジェクトを表示した後に自身を非表示）
    public void Cut()
    {
        if (hiddenObjects == null || hiddenObjects.Length == 0)
        {
            Debug.LogWarning("表示させるオブジェクトが設定されていません！");
            return;
        }

        SetObjectsActive(true);

        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                obj.transform.position = transform.position;
            }
        }

        gameObject.SetActive(false);
    }

    private void SetObjectsActive(bool isActive)
    {
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
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
