using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    SingletonIndexPlayer DataPlayer;
    Animator animator_Player;

    [SerializeField] bool isParry;
    [SerializeField] Transform bear;

    // Start is called before the first frame update
    void Start()
    {
        DataPlayer = SingletonIndexPlayer.Instance;
        animator_Player = GetComponent<Animator>();    }

    // Update is called once per frame
    void Update()
    {
        AttackController();
        ParryController();
    }

    private bool can_Attack = true;
    [SerializeField] float TimeToAttack = 1f;
    void AttackController()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator_Player.SetTrigger("Attack");
            
        }
    }

    void ParryController()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isParry = true;
            animator_Player.SetTrigger("Parry");
        }
        else
        {
            isParry = false;
        }
    }

    public void TakeDamege(float Damege)
    {
        if(isParry == true)
        {
            animator_Player.SetTrigger("Counter Attack");
            return;
        }
        DataPlayer.Health -= Damege;
        if (DataPlayer.Health < 0)
        {
            Debug.Log("Player Is Death");
        }
    }

    public void CounterAttack()
    {
        bear.GetComponent<BearAttack>().IsCounter();
    }
}
