using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    PhotonView photonView;

    SingletonIndexPlayer DataPlayer;
    Animator animator_Player;

    [SerializeField] bool isParry;
    [SerializeField] Transform bear;

    [Header("Attack")]
    private Camera cam;
    [SerializeField] float attackDistance;
    [SerializeField] LayerMask EnemyLayer;
    Color raycolor = Color.red;

    [Header("Combo Attack")]
    [SerializeField] float coolDownTime = 2f;
    [SerializeField] float nextFireTime = 0f;
    [SerializeField] static int noOfClicks = 0;
    [SerializeField] float lastClickedTime = 0;
    [SerializeField] float maxComboDelay = 1;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        DataPlayer = SingletonIndexPlayer.Instance;
        animator_Player = GetComponent<Animator>();   
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            AttackController();
            ParryController();
        }
    }

    void AttackController()
    {
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }
        if (Input.GetMouseButtonDown(0))
        {
            
            HandleAttack();
        }
    }

    void HandleAttack()
    {
        lastClickedTime = Time.time;
        noOfClicks = Mathf.Clamp(noOfClicks + 1, 0, 3);

        AnimatorStateInfo currentState = animator_Player.GetCurrentAnimatorStateInfo(0);

        if (noOfClicks == 1)
        {
            Debug.Log("Combo 1");
            animator_Player.SetTrigger("Attack1");
        }
        else if (noOfClicks == 2 && currentState.normalizedTime > 0.2f && currentState.IsName("Attack1"))
        {
            Debug.Log("Combo 2");
            animator_Player.SetTrigger("Attack2");
        }
        else if (noOfClicks == 3 && currentState.normalizedTime > 0.2f && currentState.IsName("Attack2"))
        {
            Debug.Log("Combo 3");
            animator_Player.SetTrigger("Attack3");
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
        bear.GetComponent<EnemyAttack>().IsCounter();
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
