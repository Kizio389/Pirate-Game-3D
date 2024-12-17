using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using Photon.Pun;
public class Movement : MonoBehaviour
{
    PhotonView photonView;

    private CharacterController characterController;
    private Animator animator_Player;

    [SerializeField] private float SpeedToWalk = 1f;
    [SerializeField] private float SpeedToRun = 3f;
    [SerializeField] private bool isAttack;
    private bool isRun = false;

    InformationIndexPlayer UIPlayer;
    SingletonIndexPlayer DataPlayer;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        animator_Player = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        UIPlayer = characterController.GetComponent<InformationIndexPlayer>();
        DataPlayer = SingletonIndexPlayer.Instance;
    }


    private void Update()
    {
        if (!photonView.IsMine)
        {
            MovementController();
            //Debug.Log(DataPlayer.Stamina);
            if (isRun == false)
            {
                if (DataPlayer.Stamina >= DataPlayer.Max_Stamina)
                {
                    return;
                }
                DataPlayer.Stamina += .2f;
            }
        }
    }
    void MovementController()
    {
        animator_Player.SetBool("ToWalk", false);
        animator_Player.SetBool("ToRun", false);
        animator_Player.SetBool("Right", false);
        animator_Player.SetBool("Left", false);
        float _Speed = SpeedToWalk;

        Vector3 moveDirection = Vector3.zero; // Khởi tạo hướng di chuyển

        // Di chuyển tiến/lùi
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward; // Di chuyển tiến
            animator_Player.SetBool("ToWalk", true);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRun = true;
                if (DataPlayer.Stamina <= 0)
                {
                    DataPlayer.Stamina = -1 ;
                    animator_Player.SetBool("ToWalk", true);
                }
                if(DataPlayer.Stamina > 0)
                {
                    DataPlayer.Stamina -= .05f;
                    _Speed = SpeedToRun;
                    animator_Player.SetBool("ToWalk", false);
                    animator_Player.SetBool("ToRun", true);
                }
            }
            

        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
        {
            isRun = false;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward; // Di chuyển lùi
        }

        // Di chuyển trái/phải
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right; // Di chuyển trái
            animator_Player.SetBool("Left", true);
            _Speed--;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right; // Di chuyển phải
            animator_Player.SetBool("Right", true);
            _Speed--;
        }

        // Chuẩn hóa hướng và di chuyển
        moveDirection = moveDirection.normalized;
        characterController.Move(moveDirection * _Speed * Time.deltaTime);
    }

    
}

