/*using unityengine;
using photon.pun;

public class enemyattack : monobehaviourpun
{
    public float detectionradius = 5f;
    public layermask detectionlayer;
    public float stopdistance = 1f;
    public float movespeed = 3f;

    [serializefield] private transform target;
    public float damage = 10f;

    animator animator;

    void start()
    {
        animator = getcomponent<animator>();
    }

    void update()
    {
        detectandmove();
    }

    void detectandmove()
    {
        if (target != null)
        {
            float distancetotarget = vector3.distance(transform.position, target.position);

            if (distancetotarget > stopdistance)
            {
                movetowardstarget();
            }
            else
            {
                attack();
            }
        }
        else
        {
            searchfortarget();
        }
    }

    void searchfortarget()
    {
        collider[] hitcolliders = physics.overlapsphere(transform.position, detectionradius, detectionlayer);
        if (hitcolliders.length > 0)
        {
            target = hitcolliders[0].transform;
        }
    }

    void movetowardstarget()
    {
        vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * movespeed * time.deltatime;
        animator.setbool("run", true);
    }

    void attack()
    {
        animator.setbool("run", false);
        animator.settrigger("attack");

        if (target != null && target.getcomponent<playercontroller>())
        {
            photonview.rpc("applydamagetoplayer", rpctarget.all, target.getcomponent<photonview>().viewid);
        }
    }

    [punrpc]
    void applydamagetoplayer(int playerviewid)
    {
        photonview targetphotonview = photonview.find(playerviewid);
        if (targetphotonview != null)
        {
            targetphotonview.getcomponent<playercontroller>().takedamagefromenemy(damage);
        }
    }
}
*/