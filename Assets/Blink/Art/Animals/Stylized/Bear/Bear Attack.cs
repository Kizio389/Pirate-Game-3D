using UnityEngine;

public class BearAttack : MonoBehaviour
{
    public float detectionRadius = 5f;  // Bán kính phát hiện
    public LayerMask detectionLayer;    // Layer của vật thể cần phát hiện
    public float stopDistance = 1f;     // Khoảng cách dừng
    public float moveSpeed = 3f;        // Tốc độ di chuyển
    public float outOfRangeDuration = 5f; // Thời gian mục tiêu nằm ngoài phạm vi

    public float rotationSpeed = 5f;   // Tốc độ xoay

    [SerializeField] private Transform target;           // Mục tiêu hiện tại
    private float outOfRangeTime = 0f;  // Bộ đếm thời gian khi mục tiêu rời khỏi phạm vi

    public float Damege = 10f;

    Animator animator;
    BearController bearController;
    private void Start()
    {
        animator = GetComponent<Animator>();
        bearController = GetComponent<BearController>();
    }

    void Update()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Attack", false);
        // Kiểm tra mục tiêu trong phạm vi
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Kiểm tra nếu mục tiêu vượt ra khỏi phạm vi
            if (distanceToTarget > detectionRadius)
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
            bearController.enabled = true;
            // Tìm mục tiêu mới nếu không có mục tiêu
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
            if (hitColliders.Length > 0)
            {
                target = hitColliders[0].transform; // Chọn mục tiêu đầu tiên
                Debug.Log("Detected new target: " + target.name);
            }
        }

        // Di chuyển tới mục tiêu nếu có
        if (target != null)
        {
            bearController.enabled = false;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > stopDistance)
            {
                animator.SetBool("Run", true);
                // Di chuyển đến mục tiêu
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Xoay mặt về phía mục tiêu
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Run", false);
                animator.SetBool("Attack", true);
                
            }
        }
    }

    public void Attack()
    {
        if (target == null)
        {
            return;
        }
        target.GetComponent<PlayerController>().TakeDamage(Damege);
    }

    public void IsCounter()
    {
        animator.SetTrigger("Hit");
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

