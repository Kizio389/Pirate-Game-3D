using UnityEngine;
using Photon.Pun;

public class EnemyController : MonoBehaviourPun
{
    public float Health = 200f;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void ApplyDamage(float damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
    }

    void Die()
    {
        animator.SetTrigger("isDeath");
        Destroy(gameObject, 2f); // Xóa kẻ địch sau khi chết
    }
}
