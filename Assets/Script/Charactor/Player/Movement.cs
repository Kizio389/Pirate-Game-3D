using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviourPun
{
    private CharacterController characterController;
    private Animator animator_Player;

    [SerializeField] private float SpeedToWalk = 1f;
    [SerializeField] private float SpeedToRun = 3f;
    private Transform cameraTransform;
    private float syncRate = 0.1f;
    private float syncTimer = 0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator_Player = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleMovement();

        syncTimer += Time.deltaTime;
        if (syncTimer >= syncRate)
        {
            photonView.RPC("SyncPlayerState", RpcTarget.Others, transform.position, transform.rotation);
            syncTimer = 0f;
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = cameraTransform.right * horizontal + cameraTransform.forward * vertical;
        moveDirection.y = 0;

        characterController.Move(moveDirection.normalized * (Input.GetKey(KeyCode.LeftShift) ? SpeedToRun : SpeedToWalk) * Time.deltaTime);

        animator_Player.SetFloat("MoveX", horizontal);
        animator_Player.SetFloat("MoveY", vertical);
    }

    [PunRPC]
    void SyncPlayerState(Vector3 position, Quaternion rotation)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }
}
