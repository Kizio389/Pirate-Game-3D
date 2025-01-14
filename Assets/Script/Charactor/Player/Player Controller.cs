using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerController : MonoBehaviourPun
{
    Animator animator_Player;

    private float health;
    private float maxHealth;
    
    

    private bool isAttacking = false;
    private bool isParrying = false;

    void Start()
    {
       
        

        animator_Player = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

       
        HandleAttack();
      
    }
   

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(3)) // Chuột trái để tấn công
        {
            if (!isAttacking)
            {
                StartCoroutine(PerformAttack("Attack1"));

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 2.0f)) // Phạm vi tấn công 2m
                {
                    var enemy = hit.transform.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(10); // Gây 10 sát thương
                    }
                }
            }
        }
    }

    IEnumerator PerformAttack(string attackTrigger)
    {
        isAttacking = true;
        animator_Player.SetTrigger(attackTrigger);

        yield return new WaitForSeconds(1f); // Thời gian giữa các đòn tấn công
        isAttacking = false;
    }





  

   
}
