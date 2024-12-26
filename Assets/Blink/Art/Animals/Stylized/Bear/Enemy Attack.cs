using UnityEngine;
using Photon.Pun;

public class EnemyAttack : MonoBehaviourPun
{
    public float detectionRadius = 5f;
    public LayerMask detectionLayer;
    public float stopDistance = 1f;
    public float moveSpeed = 3f;

    [SerializeField] private Transform target;
    public float Damage = 10f;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectAndMove();
    }

    void DetectAndMove()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > stopDistance)
            {
                MoveTowardsTarget();
            }
            else
            {
                Attack();
            }
        }
        else
        {
            SearchForTarget();
        }
    }

    void SearchForTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        if (hitColliders.Length > 0)
        {
            target = hitColliders[0].transform;
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        animator.SetBool("Run", true);
    }

    void Attack()
    {
        animator.SetBool("Run", false);
        animator.SetTrigger("Attack");

        if (target != null && target.GetComponent<PlayerController>())
        {
            photonView.RPC("ApplyDamageToPlayer", RpcTarget.All, target.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void ApplyDamageToPlayer(int playerViewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);
        if (targetPhotonView != null)
        {
            targetPhotonView.GetComponent<PlayerController>().TakeDamageFromEnemy(Damage);
        }
    }
}
