using UnityEngine;

public class Player_Cuted : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;   // �i�ޑ���
    [SerializeField] private float jumpForce = 10f;  // �W�����v�̋���
    [SerializeField] private KeyCode jumpKey = KeyCode.Space; // �W�����v����L�[���C���X�y�N�^�[�őI��
    [SerializeField] private GameObject objectToShow;  // �\������I�u�W�F�N�g
    [SerializeField] private Vector3 spawnPosition;  // �\���I�u�W�F�N�g�̃X�|�[���ʒu

    private Rigidbody2D rb;  // Rigidbody2D�R���|�[�l���g
    private Camera mainCamera; // ���C���J����

    void Start()
    {
        // Rigidbody2D�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody2D>();
        // ���C���J�������擾
        mainCamera = Camera.main;
    }

    void Update()
    {
        // �L�����N�^�[���E�ɐi�߂�
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // �W�����v����
        if (Input.GetKeyDown(jumpKey) && Mathf.Abs(rb.velocity.y) < 0.01f)  // �n�ʂɂ��鎞�̂݃W�����v
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // �J�����̊O�ɏo�Ȃ��悤�ɂ���i�������������R�j
        ClampPositionToCameraSidesAndTop();

        // L�L�[��Link���\�b�h�Ăяo��
       if (Input.GetKeyDown(KeyCode.O))
      {
            Link();
     }
  }

    private void ClampPositionToCameraSidesAndTop()
    {
        if (mainCamera == null) return;

        Vector3 pos = transform.position;

        // �J�����̍����ƉE��ʒu���擾
        Vector3 min = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 max = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // ���E����Clamp����
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);

        // ���������Clamp����
        if (pos.y > max.y)
        {
            pos.y = max.y;
        }

        // ���ɂ͏o�Ă������̂ŁAy���������Ȃ�͎̂��R�I

        transform.position = pos;

        // ��ʊO�ɏo���ꍇ�A�Q�[���I�[�o�[����
        if (pos.y < min.y)  // ��ʂ̉��ɏo����
        {
            StageManager.Instance.SetGameOver();
        }
    }

    public void SetPlayerObjShow(GameObject obj)
    {
        objectToShow = obj;
    }

    // Link���\�b�h�Ŏw�肵���I�u�W�F�N�g��\�����A���̌ケ�̃I�u�W�F�N�g���\���ɂ���
    public void Link()
    {
        if (objectToShow != null)
        {
            // objectToShow�̈ʒu���w�肵���I�u�W�F�N�g�̈ʒu�ɐݒ�
            objectToShow.transform.position = transform.position;  // �����Ō��݂̃v���C���[�̈ʒu�ɐݒ�
            objectToShow.SetActive(true);  // �I�u�W�F�N�g��\��
        }

        // ���̃I�u�W�F�N�g���\���ɂ���
        gameObject.SetActive(false);
    }
}
