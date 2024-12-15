using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    SingletonIndexPlayer DataPlayer;
    Animator animator_Player;

    [SerializeField] bool isParry;
    [SerializeField] Transform bear;

    void Start()
    {
        DataPlayer = SingletonIndexPlayer.Instance;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            photonView.RPC("TriggerAttack", RpcTarget.All);
        }
    }

    void HandleParry()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isParry = true;
            photonView.RPC("TriggerParry", RpcTarget.All, true);
        }
        else
        {
            isParry = false;
            photonView.RPC("TriggerParry", RpcTarget.All, false);
        }
    }

    [PunRPC]
    void TriggerAttack()
    {
        animator_Player.SetTrigger("Attack");
    }

    [PunRPC]
    void TriggerParry(bool state)
    {
        isParry = state;
        if (state) animator_Player.SetTrigger("Parry");
    }

    public void TakeDamage(float damage)
    {
        if (isParry)
        {
            photonView.RPC("CounterAttack", RpcTarget.All);
            return;
        }

        DataPlayer.Health -= damage;
        if (DataPlayer.Health <= 0)
        {
            Debug.Log("Player is Dead");
        }
    }

    [PunRPC]
    void CounterAttack()
    {
        bear.GetComponent<BearAttack>().IsCounter();
    }
}
