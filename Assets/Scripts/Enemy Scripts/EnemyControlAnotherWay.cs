﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControlAnotherWay : MonoBehaviour
{
    public Transform[] walkPoints;
    private int walkIndex;
    private Transform playerTarget;
    private Animator animator;
    private NavMeshAgent navAgent;

    private float walkDistance = 8f;
    private float attackDistance = 2f;

    private float currentAttackTime;
    private float waitAttackTime = 1f;

    private Vector3 nextDestination;

    void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > walkDistance)
        {
            if (navAgent.remainingDistance <= 0.5f)
            {
                navAgent.isStopped = false;

                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
                animator.SetInteger("Atk", 0);

                nextDestination = walkPoints[walkIndex].position;
                navAgent.SetDestination(nextDestination);

                if (walkIndex >= walkPoints.Length - 1)
                {
                    walkIndex = 0;
                }
                else
                {
                    walkIndex++;
                }
            }
        }
        else
        {
            if (distance > attackDistance)
            {
                navAgent.isStopped = false;

                animator.SetBool("Walk", false);
                animator.SetBool("Run", true);
                animator.SetInteger("Atk", 0);

                navAgent.SetDestination(playerTarget.position);
            }
            else
            {
                navAgent.isStopped = true;
                animator.SetBool("Run", false);

                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y,
                    playerTarget.position.z);

                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(targetPosition - transform.position), 5f * Time.deltaTime);

                if (currentAttackTime >= waitAttackTime)
                {
                    int attackRange = Random.Range(1, 3);
                    animator.SetInteger("Atk", attackRange);
                    currentAttackTime = 0;
                }
                else
                {
                    animator.SetInteger("Atk", 0);
                    currentAttackTime += Time.deltaTime;
                }
            }
        }
    }
}
