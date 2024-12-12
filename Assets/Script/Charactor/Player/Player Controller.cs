using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    SingletonIndexPlayer DataPlayer;
    Animator animator_Player;

    [SerializeField] bool isParry;
    [SerializeField] Transform bear;

    [Header("Attack")]
    private Camera cam;
    [SerializeField] float attackDistance;
    [SerializeField] LayerMask EnemyLayer;
    Color raycolor = Color.red;
    // Start is called before the first frame update
    void Start()
    {
        DataPlayer = SingletonIndexPlayer.Instance;
        animator_Player = GetComponent<Animator>();   
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * attackDistance, raycolor, attackDistance);
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
            AttackRaycast();
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

    public void CounterAttack()
    {
        bear.GetComponent<BearAttack>().IsCounter();
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

    void AttackRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward * attackDistance, out RaycastHit hit, attackDistance, EnemyLayer))
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Hit Enemy");
            }
        }
    } 
}
