using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderUpdate : MonoBehaviour
{

    [SerializeField] SkinnedMeshRenderer _SkinnedMeshRenderer;
    [SerializeField] MeshCollider _Collider;
    // Start is called before the first frame update
    void Start()
    {
        _SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _Collider.GetComponent<MeshCollider>().sharedMesh = _SkinnedMeshRenderer.sharedMesh;
    }

    private float time = 0;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 0.0167f) //Cập nhật collider mới, thời gian tương ứng 1 với 60FPS (Nếu frame của animation thấp/cao hơn thì chỉnh sửa thời gian)
        {
            time = 0;
            UpdateCollider();
        }
    }


    //Update collider tương ứng với 1 mesh của frame hiện tại
    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        _SkinnedMeshRenderer.BakeMesh(colliderMesh); //Tạo mesh mới từ mesh hiện tại
        _Collider.sharedMesh = null; //Collider rỗng
        _Collider.sharedMesh = colliderMesh; //Gán mesh tĩnh vừa tạo ở trên cho collider
    }
}