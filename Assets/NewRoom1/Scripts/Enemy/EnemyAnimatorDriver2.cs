using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorDriver2 : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;

    void Start()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        bool moving = agent.velocity.magnitude > 0.5f && !agent.isStopped;
        anim.SetBool("IsMoving", moving);
    }
}