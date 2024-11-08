using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player;                  // Nhân vật cần kiểm tra
    [SerializeField] private LayerMask obstacleLayer;           // Layer của các vật cản (như tường)
    [SerializeField] private Material transparentMaterial;      // Material trong suốt
    [SerializeField] private float coneAngle = 30f;             // Góc mở của hình nón cụt (tính từ tâm)
    [SerializeField] private float maxDistance = 10f;           // Khoảng cách tối đa của hình nón cụt
    [SerializeField] private float topRadius = 0.5f;            // Bán kính trên cùng của hình nón cụt
    [SerializeField] private float bottomRadius = 1f;          // Bán kính dưới cùng của hình nón cụt

    private List<Renderer> transparentObjects = new List<Renderer>(); // Danh sách các vật đã bị thay material
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>(); // Material gốc của các vật cản

    void Update()
    {
        CheckObstacles();
    }

    private void CheckObstacles()
    {
        // Đặt lại material cho các vật cản trước đó
        ResetMaterials();

        // Tính toán hướng và khoảng cách tới player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Chỉ kiểm tra nếu player nằm trong phạm vi tối đa
        if (distanceToPlayer > maxDistance) return;

        // Tính toán các điểm viền của hình nón
        Vector3 apex = transform.position; // Đỉnh của hình nón
        Vector3 forward = directionToPlayer * maxDistance;

        // Kiểm tra các collider trong phạm vi hình nón
        foreach (Collider hit in Physics.OverlapSphere(transform.position, maxDistance, obstacleLayer))
        {
            Renderer renderer = hit.GetComponent<Renderer>();
            if (renderer == null) continue;

            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(directionToPlayer, directionToTarget);

            // Chỉ xử lý nếu vật thể nằm trong góc mở của hình nón cụt và nằm trong khoảng cách
            if (angleToTarget <= coneAngle / 2 && IsInCone(hit.transform.position, apex, forward))
            {
                if (!originalMaterials.ContainsKey(renderer))
                {
                    // Lưu lại material gốc và thay thế bằng material trong suốt
                    originalMaterials[renderer] = renderer.materials;
                    SetTransparentMaterial(renderer);
                    transparentObjects.Add(renderer);
                }
            }
        }
    }

    // Kiểm tra xem vị trí có nằm trong hình nón cụt hay không
    private bool IsInCone(Vector3 position, Vector3 apex, Vector3 direction)
    {
        Vector3 toPosition = position - apex;
        float height = direction.magnitude;
        float distance = toPosition.magnitude;
        if (distance > height) return false; // Nếu ngoài phạm vi chiều cao của nón

        float angleToPosition = Vector3.Angle(direction.normalized, toPosition.normalized);
        return angleToPosition <= coneAngle / 2; // Kiểm tra góc
    }

    // Thay đổi material của vật thể sang material trong suốt
    private void SetTransparentMaterial(Renderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length];
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = transparentMaterial;
        }
        renderer.materials = newMaterials;
    }

    // Đặt lại material gốc cho các vật thể trước đó
    private void ResetMaterials()
    {
        foreach (Renderer renderer in transparentObjects)
        {
            if (originalMaterials.TryGetValue(renderer, out Material[] originalMats))
            {
                renderer.materials = originalMats;
            }
        }
        transparentObjects.Clear();
        originalMaterials.Clear();
    }

    // Vẽ Gizmos để hiển thị hình nón cụt trong Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Màu đỏ nhạt để dễ nhận biết

        // Tính toán các đỉnh của hình nón cụt
        Vector3 apex = transform.position; // Đỉnh của hình nón
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 forward = directionToPlayer * maxDistance;

        // Các điểm trên viền của đáy hình nón cụt
        List<Vector3> bottomCircle = new List<Vector3>();

        // Số lượng điểm trên hình nón
        int segments = 20;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            Vector3 pointOnBottom = apex + forward + new Vector3(Mathf.Cos(angle) * bottomRadius, -maxDistance, Mathf.Sin(angle) * bottomRadius);
            bottomCircle.Add(pointOnBottom);
        }

        // Vẽ các cạnh của hình nón cụt
        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(apex, bottomCircle[i]);
            Gizmos.DrawLine(bottomCircle[i], bottomCircle[(i + 1) % segments]);
        }
    }
}
