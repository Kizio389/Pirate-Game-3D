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
        // Lấy giá trị từ PlayerPrefs trong hàm Start
        health = PlayerPrefsManager.GetHealth();
        maxHealth = PlayerPrefsManager.GetMaxHealth();
        

        animator_Player = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

       
        HandleAttack();
        HandleParry();
    }
   

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                StartCoroutine(PerformAttack("Attack1"));
            }
        }
    }

    IEnumerator PerformAttack(string attackTrigger)
    {
        isAttacking = true;
        animator_Player.SetTrigger(attackTrigger);

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    void HandleParry()
    {
        if (Input.GetMouseButton(1))
        {
            if (!isParrying)
            {
                isParrying = true;
                photonView.RPC("TriggerParry", RpcTarget.All, true);
            }
        }
        else if (isParrying)
        {
            isParrying = false;
            photonView.RPC("TriggerParry", RpcTarget.All, false);
        }
    }

    void TriggerParry(bool state)
    {
        animator_Player.SetBool("Parry", state);
    }

    public void TakeDamage(float damage)
    {
        if (isParrying)
        {
            Debug.Log("Damage blocked by Parry!");
            return;
        }

        health -= damage;
        PlayerPrefsManager.SetHealth(Mathf.Max(health, 0));

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamageFromEnemy(float damage)
    {
        if (!photonView.IsMine) return;

        photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
    }

    void Die()
    {
        animator_Player.SetTrigger("Die");
        enabled = false;
    }
}
