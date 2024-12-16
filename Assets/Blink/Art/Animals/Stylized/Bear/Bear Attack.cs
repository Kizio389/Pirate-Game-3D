using Photon.Pun; // Import Photon để hỗ trợ multiplayer
using UnityEngine;

public class BearAttack : MonoBehaviourPun
{
    public float detectionRadius = 5f;  // Bán kính phát hiện mục tiêu
    public LayerMask detectionLayer;    // Layer của mục tiêu
    public float stopDistance = 1f;     // Khoảng cách để dừng di chuyển và tấn công
    public float moveSpeed = 3f;        // Tốc độ di chuyển về phía mục tiêu
    public float outOfRangeDuration = 5f; // Thời gian chờ khi mục tiêu nằm ngoài phạm vi

    public float rotationSpeed = 5f;    // Tốc độ xoay để hướng tới mục tiêu
    public float damage = 10f;          // Sát thương gây ra

    private Transform target;           // Mục tiêu hiện tại
    private float outOfRangeTime = 0f;  // Bộ đếm khi mục tiêu rời khỏi phạm vi
    private Animator animator;          // Quản lý hoạt ảnh
    private BearController bearController; // Điều khiển cơ bản khi không có mục tiêu

    private void Start()
    {
        animator = GetComponent<Animator>();
        bearController = GetComponent<BearController>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return; // Đảm bảo chỉ xử lý trên con gấu thuộc quyền sở hữu của người chơi hiện tại

        HandleTargetDetection(); // Phát hiện và theo dõi mục tiêu
        HandleMovementAndAttack(); // Xử lý di chuyển và tấn công
    }

    private void HandleTargetDetection()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > detectionRadius) // Nếu mục tiêu vượt phạm vi phát hiện
            {
                outOfRangeTime += Time.deltaTime;
                if (outOfRangeTime >= outOfRangeDuration)
                {
                    Debug.Log("Target lost. Returning to idle state.");
                    target = null;  // Đặt lại mục tiêu
                    outOfRangeTime = 0f; // Reset bộ đếm
                }
            }
            else
            {
                outOfRangeTime = 0f; // Reset bộ đếm nếu mục tiêu quay lại
            }
        }
        else
        {
            bearController.enabled = true; // Kích hoạt BearController khi không có mục tiêu
            // Tìm mục tiêu mới
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
            if (hitColliders.Length > 0)
            {
                target = hitColliders[0].transform; // Chọn mục tiêu đầu tiên
                Debug.Log("Detected new target: " + target.name);
            }
        }
    }

    private void HandleMovementAndAttack()
    {
        if (target != null)
        {
            bearController.enabled = false; // Tắt BearController khi đang theo dõi mục tiêu
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > stopDistance)
            {
                animator.SetBool("Run", true);
                MoveTowardsTarget(); // Di chuyển về phía mục tiêu
            }
            else
            {
                animator.SetBool("Run", false);
                animator.SetBool("Attack", true); // Chuyển sang trạng thái tấn công
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized; // Tính hướng di chuyển
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime); // Di chuyển mượt mà

        // Xoay mặt về phía mục tiêu
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime); // Xoay mượt mà
    }

    public void Attack()
    {
        if (target == null) return; // Không thực hiện nếu không có mục tiêu

        // Gây sát thương lên mục tiêu
        photonView.RPC("ApplyDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void ApplyDamage(float damage)
    {
        if (target != null)
        {
            target.GetComponent<PlayerController>().TakeDamage(damage); // Gọi hàm nhận sát thương trên PlayerController
        }
    }

    public void IsCounter()
    {
        animator.SetTrigger("Hit"); // Phát hoạt ảnh khi bị phản đòn
    }

    private void OnDrawGizmos()
    {
        // Vẽ bán kính phát hiện trong Scene
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Vẽ khoảng cách dừng trong Scene
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
