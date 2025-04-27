using UnityEngine;
using TMPro; // TextMeshPro���g�����߂ɕK�v
using System.Collections.Generic;
using System.Net.Http.Headers; // List<T> ���g�����߂ɕK�v

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float normalJumpForce = 10f;
    [SerializeField] private float highJumpForce = 18f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("��\���ɂ��Ă����I�u�W�F�N�g4��")]
    [SerializeField] private GameObject[] hiddenObjects = new GameObject[4];

    [Header("TextMeshPro�̎Q��")]
    [SerializeField] private TextMeshProUGUI keyBindingsText; // TextMeshProUGUI���g���ꍇ

    [Header("�W�����v�̌��ʉ�")]
    [SerializeField] private AudioClip jumpSound; // �W�����v���̉�
    private AudioSource audioSource; // AudioSource���i�[

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

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (audioSource == null) audioSource = GetComponent<AudioSource>(); // AudioSource���擾

        originalScale = transform.localScale;

        if (boxCollider != null)
        {
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }

        SetObjectsActive(false);
    }

    void Update()
    {
        isGrounded = CheckGroundedByCollider();

        float move = 0f;
        if (Input.GetKey(moveLeftKey)) move = -1f;
        if (Input.GetKey(moveRightKey)) move = 1f;

        if (!isCrouching)
        {
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            float jumpForce = normalJumpForce;

            if (isCrouching)
            {
                float crouchDuration = Time.time - crouchStartTime;
                if (crouchDuration >= 0.7f)
                {
                    jumpForce = highJumpForce;
                }
                isCrouching = false;
                ResetCrouch();
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            PlayJumpSound(); // �W�����v�����Đ�
        }

        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            crouchStartTime = Time.time;
            StartCrouch();
        }

        if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
            ResetCrouch();
        }

        ClampPositionAndCheckFall();
    }

    private bool CheckGroundedByCollider()
    {
        Bounds bounds = boxCollider.bounds;
        Vector2 bottomCenter = new Vector2(bounds.center.x, bounds.min.y - 0.05f);
        Vector2 checkSize = new Vector2(bounds.size.x * 0.9f, 0.02f);
        return Physics2D.OverlapBox(bottomCenter, checkSize, 0f, groundLayer);
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
                Debug.LogWarning("StageManager.Instance �����݂��܂���I");
            }
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

    public void Cut()
    {
        if (hiddenObjects == null || hiddenObjects.Length == 0)
        {
            Debug.LogWarning("�\��������I�u�W�F�N�g���ݒ肳��Ă��܂���I");
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

        // ShuffleKeys(); �� Cut�ł̓V���b�t�����Ȃ�

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

    public void SetKey(KeyCode left, KeyCode right, KeyCode jump, KeyCode crouch)
    {
        moveLeftKey = left;
        moveRightKey = right;
        jumpKey = jump;
        crouchKey = crouch;
    }

    public void ShuffleKeys()  // public �ɕύX
    {
        List<KeyCode> keys = new List<KeyCode> { KeyCode.A, KeyCode.F, KeyCode.J, KeyCode.L };
        for (int i = 0; i < keys.Count; i++)
        {
            KeyCode temp = keys[i];
            int randomIndex = Random.Range(i, keys.Count);
            keys[i] = keys[randomIndex];
            keys[randomIndex] = temp;
        }

        moveLeftKey = keys[0];
        moveRightKey = keys[1];
        jumpKey = keys[2];
        crouchKey = keys[3];

        Debug.Log($"�L�[���V���b�t�����܂����I ���ړ�: {moveLeftKey}, �E�ړ�: {moveRightKey}, �W�����v: {jumpKey}, ���Ⴊ��: {crouchKey}");
    }

    public (KeyCode moveLeft, KeyCode moveRight, KeyCode jump, KeyCode crouch) GetCurrentKeyBindings()
    {
        return (moveLeftKey, moveRightKey, jumpKey, crouchKey);
    }

    public KeyCode GetKeyByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return moveLeftKey;
            case 1:
                return moveRightKey;
            case 2:
                return jumpKey;
            case 3:
                return crouchKey;
            default:
                return KeyCode.None;
        }
    }

    // �W�����v���̉����Đ����郁�\�b�h
    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }
}
