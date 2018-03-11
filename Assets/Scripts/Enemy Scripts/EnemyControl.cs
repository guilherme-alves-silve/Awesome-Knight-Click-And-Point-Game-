using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Walk,
    Run,
    Attack,
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
    private bool finishedAnimation = true;
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

        enemyCurrentState = SetEnemyState(enemyCurrentState, enemyLastState, enemyToPlayerDistance);

        if (finishedMovement)
        {
            GetStateControl(enemyCurrentState);
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
                else if (stateInfo.IsTag("Atk1") || stateInfo.IsTag("Atk2"))
                {
                    animator.SetInteger("Atk", 0);
                }
            }
        }
    }

    EnemyState SetEnemyState(EnemyState currentState, EnemyState lastState, float enemyToPlayerDistance)
    {
        enemyToPlayerDistance = Vector3.Distance(transform.position, playerTarget.position);
        float initialDistance = Vector3.Distance(initialPosition, transform.position);

        if (initialDistance > followDistance)
        {
            lastState = currentState;
            currentState = EnemyState.GoBack;
        }
        else if (enemyToPlayerDistance <= attackDistance)
        {
            lastState = currentState;
            currentState = EnemyState.Attack;
        }
        else if ((enemyToPlayerDistance >= alertAttackDistance) &&
                 (lastState == EnemyState.Pause) ||
                 (lastState == EnemyState.Attack))
        {
            lastState = currentState;
            currentState = EnemyState.Pause;
        }
        else if ((enemyToPlayerDistance <= alertAttackDistance) &&
                 (enemyToPlayerDistance > attackDistance))
        {
            if ((currentState != EnemyState.GoBack) ||
                (EnemyState.Walk == lastState))
            {
                lastState = currentState;
                currentState = EnemyState.Pause;
            }
        }
        else if ((enemyToPlayerDistance > alertAttackDistance) &&
                  (lastState != EnemyState.GoBack) &&
                  (lastState != EnemyState.Pause))
        {
            lastState = currentState;
            currentState = EnemyState.Walk;
        }

        return currentState;
    }

    void GetStateControl(EnemyState currentState)
    {
        if ((EnemyState.Run == currentState) ||
            (EnemyState.Pause == currentState))
        {
            if (currentState != EnemyState.Attack)
            {
                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, 
                    playerTarget.position.z);

                if (Vector3.Distance(transform.position, targetPosition) >= 2.1f)
                {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", true);
                    navAgent.SetDestination(targetPosition);
                }
            }
        }
        else if (EnemyState.Attack == currentState)
        {
            animator.SetBool("Run", false);
            whereToMove.Set(0f, 0f, 0f);
            navAgent.SetDestination(transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(playerTarget.position - transform.position), 5f * Time.deltaTime);

            if (currentAttackTime >= waitAttackTime)
            {
                int attackRange = Random.Range(1, 3);
                animator.SetInteger("Atk", attackRange);
                finishedAnimation = false;
                currentAttackTime = 0f;
            }
            else
            {
                animator.SetInteger("Atk", 0);
                currentAttackTime += Time.deltaTime;
            }
        }
        else if (EnemyState.GoBack == currentState)
        {
            animator.SetBool("Run", true);
            Vector3 targetPosition = new Vector3(initialPosition.x, transform.position.y,
                initialPosition.z);
            navAgent.SetDestination(targetPosition);

            if (Vector3.Distance(targetPosition, initialPosition) <= 3.5f)
            {
                enemyLastState = currentState;
                currentState = EnemyState.Walk;
            }
        }
        else if (EnemyState.Walk == currentState)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Walk", true);

            if (Vector3.Distance(transform.position, whereToNavigate) <= 2f)
            {
                whereToNavigate.x = Random.Range(initialPosition.x - 5f, initialPosition.x + 5f);
                whereToNavigate.z = Random.Range(initialPosition.z - 5f, initialPosition.z + 5f);
            }
            else
            {
                navAgent.SetDestination(whereToNavigate);
            }
        }
        else
        {
            animator.SetBool("Rum", false);
            animator.SetBool("Walk", false);
            whereToMove.Set(0f, 0f, 0f);
            navAgent.isStopped = true;
        }
    }
}
