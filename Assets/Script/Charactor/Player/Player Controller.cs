using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerController : MonoBehaviourPun
{
    Animator animator_Player;

    private float health;
    private float maxHealth;
    private float currentStamina;
    private float maxStamina;
    private float staminaRegenRate = 5f;

    private bool isAttacking = false;
    private bool isParrying = false;

    void Start()
    {
        // Lấy giá trị từ PlayerPrefs trong hàm Start
        health = PlayerPrefsManager.GetHealth();
        maxHealth = PlayerPrefsManager.GetMaxHealth();
        currentStamina = PlayerPrefsManager.GetStamina();
        maxStamina = PlayerPrefsManager.GetMaxStamina();

        animator_Player = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleMovement();
        HandleAttack();
        HandleParry();
    }
    [PunRPC]
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = horizontal != 0 || vertical != 0;
        bool canRun = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;

        float speed = canRun ? 3f : 1f;

        if (canRun)
        {
            currentStamina -= 1f * Time.deltaTime;
            PlayerPrefsManager.SetStamina(currentStamina);
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            PlayerPrefsManager.SetStamina(currentStamina);
        }

        animator_Player.SetBool("Run", canRun);
        animator_Player.SetBool("ToWalk", isMoving && !canRun);
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

    [PunRPC]
    void TriggerParry(bool state)
    {
        animator_Player.SetBool("Parry", state);
    }

    [PunRPC]
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
