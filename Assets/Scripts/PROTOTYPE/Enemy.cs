using UnityEngine;
using Pathfinding;
using UnityEngine.Events;

public class Enemy : Hittable
{
    public Animator anim;
    public Rigidbody rb;
    public Transform lookAt;

    public GameObject[] itemDrops;

    public TransformAnchor playerTransformAnchor = default;
    public AIDestinationSetter AI;

    public LayerMask detectionObstacles;

    public bool detected = true;
    public float detectionRange;

    bool died;

    public UnityEvent<Enemy> OnEnemyDied = new UnityEvent<Enemy>();
    public UnityEvent<Enemy> OnTakeDigDamage = new UnityEvent<Enemy>();

    public GameObject hitParticle;

    
    void OnEnable()
    {
        playerTransformAnchor.OnAnchorProvided += AssignAITarget;
        if (playerTransformAnchor.isSet)
            AssignAITarget();
    }

    void OnDisable()
    { 
        playerTransformAnchor.OnAnchorProvided -= AssignAITarget;
    }

    void AssignAITarget()
    {
        if (AI != null)
        {
            AI.target = playerTransformAnchor.Value;
        }
    }

    void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 3);
        }
    }

    override public void Hit(DamageInfo damageInfo)
    {
        base.Hit(damageInfo);
        if (damageInfo.DamageType == DamageType.Dig)
            OnTakeDigDamage.Invoke(this);
        if (health > 0)
        {
            rb.AddForce(damageInfo.Direction * damageInfo.Knockback);
            lookAt.forward = damageInfo.Direction;
            Instantiate(hitParticle, transform.position, lookAt.rotation);

            anim.SetTrigger("Hit");
        }
        else if (!died)
        {
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("Die");

        //spawn a hit particle in the opposite direction of the player;
        Vector3 hitDirection = playerTransformAnchor.Value.position - transform.position;
        lookAt.forward = -hitDirection;
        Instantiate(hitParticle, transform.position, lookAt.rotation);

        //item drops
        if (itemDrops.Length > 0)
        {
            for (int i = 0; i < itemDrops.Length; i++)
            {
                Instantiate(itemDrops[i], transform.position, Quaternion.identity);
            }
        }
        OnEnemyDied.Invoke(this);
        //give 0.5 seconds for the death animation to happen before disabling the object
        Invoke("Delete", 0.5f);
        died = true;
    }

    void Delete()
    {
        gameObject.SetActive(false);
    }
}