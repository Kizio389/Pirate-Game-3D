using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator_Player;

    [SerializeField] private float SpeedToWalk = 1f;
    [SerializeField] private float SpeedToRun = 3f;
    [SerializeField] private bool isAttack;

    InformationIndexPlayer UIPlayer;
    SingletonIndexPlayer DataPlayer;
    private void Awake()
    {
        animator_Player = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        UIPlayer = characterController.GetComponent<InformationIndexPlayer>();
        DataPlayer = SingletonIndexPlayer.Instance;
    }


    private void Update()
    {
        
        if (isAttack == false)
        {
            MovementController();
        }
        Debug.Log(DataPlayer.Stamina);
        if(animator_Player.GetBool("ToRun") == false)
        {
            if(DataPlayer.Stamina >= DataPlayer.Max_Stamina)
            {
                return;
            }
            DataPlayer.Stamina += .2f;
        }
        AttackController();
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
                DataPlayer.Stamina -= .05f;
                if (DataPlayer.Stamina <= 0)
                {
                    DataPlayer.Stamina = 0;
                    _Speed = SpeedToWalk;
                    animator_Player.SetBool("ToWalk", true);
                    animator_Player.SetBool("ToRun", false);
                }
                else if (DataPlayer.Stamina > 0)
                {
                    _Speed = SpeedToRun;
                    animator_Player.SetBool("ToWalk", false);
                    animator_Player.SetBool("ToRun", true);
                }
            }
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

    private bool can_Attack = true;
    [SerializeField] float TimeToAttack = 1f;
    void AttackController()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && can_Attack == true)
        {
            can_Attack = false;
            animator_Player.SetTrigger("Attack");
            StartCoroutine(CooldownAttack());
        }
    }

    IEnumerator CooldownAttack()
    {
        float timer = 0f;
        while(timer < TimeToAttack)
        {
            timer += Time.deltaTime;
            if(timer >= TimeToAttack)
            {
                can_Attack = true;
            }
        }
        yield return null;
    }
}

