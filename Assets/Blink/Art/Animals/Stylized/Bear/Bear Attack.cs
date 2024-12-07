using UnityEngine;

public class SphereCastChase : MonoBehaviour
{
    public float detectionRadius = 5f;       // Bán kính phát hiện
    public float attackRange = 1.5f;        // Khoảng cách để kích hoạt tấn công
    public LayerMask playerLayer;           // Layer của Player
    //public Animator animator;               // Animator để kích hoạt hoạt ảnh

    [SerializeField] private Transform player;               // Tham chiếu đến Player

    BearController controller;
    void Update()
    {
        // Kiểm tra Player trong phạm vi SphereCast
        CheckForPlayer();

        // Nếu có Player, di chuyển về phía họ
        if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    private void CheckForPlayer()
    {
        RaycastHit hit;

        // Sử dụng SphereCast để kiểm tra Player trong bán kính
        if (Physics.SphereCast(transform.position, detectionRadius, transform.forward, out hit, detectionRadius, playerLayer))
        {
            Debug.Log("Hit player");
            // Nếu va chạm với Player, lưu tham chiếu
            if (hit.collider.CompareTag("Player"))
            {
                player = hit.collider.transform;
                Debug.Log("Phát hiện Player: " + player.name);
            }
        }
        else
        {
            player = null; // Không phát hiện thấy Player
        }

        // Debug hiển thị SphereCast trong Scene View
        Debug.DrawRay(transform.position, transform.forward * detectionRadius, Color.red);
    }

    private void MoveTowardsPlayer()
    {
        // Tính khoảng cách tới Player
        float distance = Vector3.Distance(transform.position, player.position);

        // Nếu trong phạm vi tấn công, kích hoạt hoạt ảnh Attack
        if (distance <= attackRange)
        {
            //animator.SetTrigger("Attack");
            Debug.Log("Tấn công Player!");
        }
        else
        {
            // Nếu ngoài phạm vi tấn công, di chuyển về phía Player
            Vector3 direction = (player.position - transform.position).normalized;
            controller.Speed = controller.Speed * 1.5f;
            transform.position += direction * controller.Speed * Time.deltaTime;
            transform.LookAt(player); // Quay mặt về phía Player
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, detectionRadius);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
