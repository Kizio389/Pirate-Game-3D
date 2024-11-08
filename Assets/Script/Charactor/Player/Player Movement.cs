using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Index Speed")]
    [SerializeField] private float _Speed_toWalk;
    [SerializeField] private float _Speed_toRun;
    [SerializeField] private float _speedRotation;
    [SerializeField] private bool _isRun = false;
    [SerializeField] private bool _isWalk = true;

    private Animator animator_Player;
    private CharacterController characterController;

    private void Awake()
    {
        animator_Player = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float rotationSpeed = 5.0f; // Tốc độ xoay mượt mà
        float speedValue = 0f; // Giá trị tốc độ cho animation
        float _Speed = _Speed_toWalk;

        animator_Player.SetFloat("Speed_Animation", speedValue);
        animator_Player.SetBool("Left", false);
        animator_Player.SetBool("Right", false);
        _isRun = false;
        
        if (Input.GetKey(KeyCode.W))
        {
            speedValue = 1f; // Đang di chuyển về phía trước
            animator_Player.SetFloat("Speed_Animation", speedValue);
            if (Input.GetKey(KeyCode.LeftShift) && speedValue != 0)
            {
                _isRun = true;
                _isWalk = false;
                _Speed = _Speed_toRun;
                animator_Player.SetBool("isRun", _isRun);
                animator_Player.SetBool("isWalk", _isWalk);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            speedValue = -1f; // Đang di chuyển về phía sau
            animator_Player.SetFloat("Speed_Animation", speedValue);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (speedValue == 0f)
            {
                new WaitForSeconds(1);
                animator_Player.SetBool("Left", true);
            }
            else
            {
                transform.Rotate(new Vector3(0, -_speedRotation, 0), Space.Self);
            }   
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (speedValue == 0f)
            {
                new WaitForSeconds(1);
                animator_Player.SetBool("Right", true);
            }
            else
            {
                transform.Rotate(new Vector3(0, _speedRotation, 0), Space.Self);
            }
        }
        
        
        animator_Player.SetBool("isRun", _isRun);
        animator_Player.SetBool("isWalk", _isWalk);


        // Thiết lập giá trị Speed_Animation một cách mượt mà
        //float currentSpeed = animator_Player.GetFloat("Speed_Animation");
        //animator_Player.SetFloat("Speed_Animation", Mathf.Lerp(currentSpeed, Mathf.Abs(speedValue), Time.deltaTime * rotationSpeed));
        //animator_Player.SetFloat("Speed Animation", speedValue);
        // Di chuyển nhân vật theo hướng đã xoay
        characterController.Move(transform.forward * speedValue * Mathf.Abs(speedValue) * _Speed * Time.deltaTime);
    }
}
