using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển của mũi tên
    public float followDistance = 3f; // Khoảng cách cố định mũi tên di chuyển trước người chơi
    public float stopDistance = 2f; // Khoảng cách tối thiểu để mũi tên dừng di chuyển khi người chơi đi ngược
    private Transform target; // Mục tiêu mà mũi tên sẽ chỉ đến
    private Transform player; // Vị trí của người chơi

    void Update()
    {
        if (target != null && player != null)
        {
            // Tính toán khoảng cách từ mũi tên đến người chơi
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Nếu người chơi di chuyển ngược lại và khoảng cách nhỏ hơn stopDistance, mũi tên dừng lại
            if (distanceToPlayer < stopDistance)
            {
                Debug.Log("Arrow stops moving because the player is too close and moving away.");
                return; // Dừng di chuyển mũi tên
            }

            // Tính toán khoảng cách từ người chơi đến mục tiêu
            Vector3 playerToTarget = target.position - player.position;
            playerToTarget.y = 0; // Bỏ qua trục Y để tránh sai lệch độ cao

            // Xác định vị trí lý tưởng của mũi tên so với người chơi
            Vector3 desiredPosition = player.position + playerToTarget.normalized * followDistance;

            // Di chuyển mũi tên đến vị trí desiredPosition
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, moveSpeed * Time.deltaTime);

            // Hướng mũi tên về phía mục tiêu
            Vector3 direction = (target.position - transform.position).normalized;
            transform.forward = direction;
        }
    }

    // Phương thức để thiết lập mục tiêu và người chơi
    public void SetTarget(Transform targetLocation)
    {
        target = targetLocation;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}
