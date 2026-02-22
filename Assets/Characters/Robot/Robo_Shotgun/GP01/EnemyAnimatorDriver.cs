using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorDriver : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;

    void Reset()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();

        // Play spawn once at start
        anim.SetTrigger("Spawn");
    }

    void Update()
    {
        // If agent is moving, set IsMoving true
        bool moving = agent.velocity.magnitude > 0.1f;
        anim.SetBool("IsMoving", moving);
    }
}
