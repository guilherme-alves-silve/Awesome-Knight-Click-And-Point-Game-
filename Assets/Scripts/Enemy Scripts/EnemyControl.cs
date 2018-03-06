using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Walk,
    Run,
    Pause,
    GoBack,
    Death
}

public class EnemyControl : MonoBehaviour
{

    private float attackDistance = 1.5f;
    private float alertAttackDistance = 8f;
    private float followDistance = 15f;
    private float enemyToPlayerDistance;

    [HideInInspector]
    public EnemyState enemyCurrentState = EnemyState.Idle;
    private EnemyState enemyLastState = EnemyState.Idle;

    private Transform playerTarget;
    private Vector3 initialPosition;

    private float moveSpeed = 2f;
    private float walkSpeed = 1f;

    private CharacterController characterController;
    private Vector3 whereToMove = Vector3.zero;

    private float currentAttackTime;
    private float waitAttackTime = 1f;

    private Animator animator;
    private bool finishedAnivation = true;
    private bool finishedMovement = true;

    private NavMeshAgent navAgent;
    private Vector3 whereToNavigate;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        initialPosition = transform.position;
        whereToNavigate = transform.position;
    }

    void Update()
    {
        if (EnemyState.Death == enemyCurrentState)
        {
            animator.SetBool("Death", true);
            characterController.enabled = false;
            navAgent.enabled = false;

            if (!animator.IsInTransition(0))
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Death") && 
                    (stateInfo.normalizedTime >= 0.95f))
                {
                    Destroy(gameObject, 2f);
                }
            }

            return;
        }

        //enemyCurrentState = SetEnemyState();

        if (finishedMovement)
        {
            //GetStateControl(enemyCurrentState);
        }
        else
        {
            if (!animator.IsInTransition(0))
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Idle"))
                {
                    finishedMovement = true;
                }
                else if (stateInfo.IsName("Atk1") || stateInfo.IsName("Atk2"))
                {
                    animator.SetInteger("Atk", 0);
                }
            }
        }
    }
}
