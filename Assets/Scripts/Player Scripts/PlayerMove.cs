using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    private const int LeftMouse = 0;

    private Animator animator;
    private CharacterController controller;
    private CollisionFlags collisionFlags = CollisionFlags.None;

    private float moveSpeed = 5f;
    private bool canMove;
    private bool finishedMovement = true;

    private Vector3 targetPos = Vector3.zero;
    private Vector3 playerMove = Vector3.zero;
    private float gravity = 9.8f;
    private float height;

    private float playerToDistance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateHeight();
        CheckIfFinishedMovement();
    }

    void CheckIfFinishedMovement()
    {
        if (!finishedMovement)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animator.IsInTransition(0) &&
                !stateInfo.IsName("Stand") &&
                stateInfo.normalizedTime >= 0.8f)
            {
                finishedMovement = true;
            }
        }
        else
        {
            MoveThePlayer();
            playerMove.y = height * Time.deltaTime;
            collisionFlags = controller.Move(playerMove);
        }
    }

    bool IsGrounded()
    {
        return CollisionFlags.CollidedBelow == collisionFlags;
    }

    void CalculateHeight()
    {
        if (IsGrounded())
        {
            height = 0f;
        }
        else
        {
            height -= gravity * Time.deltaTime;
        }
    }

    void MoveThePlayer()
    {
        if (Input.GetMouseButtonDown(LeftMouse))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider)
                {
                    playerToDistance = Vector3.Distance(transform.position, hit.point);

                    if (playerToDistance >= 1.0f)
                    {
                        canMove = true;
                        targetPos = hit.point;
                    }
                }
            }
        }

        if (canMove)
        {
            animator.SetFloat("Walk", 1.0f);

            Vector3 targetTemp = new Vector3(targetPos.x, transform.position.y, targetPos.z);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(targetTemp - transform.position),
                15.0f * Time.deltaTime);

            playerMove = transform.forward * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
            {
                canMove = false;
            }
        }
        else
        {
            playerMove.Set(0f, 0f, 0f);
            animator.SetFloat("Walk", 0f);
        }
    }

    public bool FinishedMovement
    {
        get
        {
            return finishedMovement;
        }
        set
        {
            finishedMovement = value;
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            return targetPos;
        }
        set
        {
            targetPos = value;
        }
    }
}
