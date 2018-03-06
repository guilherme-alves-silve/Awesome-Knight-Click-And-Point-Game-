using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public Image[] fillWait = new Image[6];
    private bool[] fadeImages = new bool[6];
    private float[] fadeTimeout = { 1.0f, 0.7f, 0.1f, 0.2f, 0.3f, 0.08f };
    private Animator animator;
    private bool canAttack = true;
    private PlayerMove playerMove;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        canAttack = !animator.IsInTransition(0) &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Stand");

        CheckToFade();
        CheckInput();
    }

    void CheckToFade()
    {
        for (int i = 0; i < 6; ++i)
        {
            if (fadeImages[i] && FadeAndWait(fillWait[i], 1.0f))
            {
                fadeImages[i] = false;
            }
        }
    }

    void CheckInput()
    {
        if (animator.GetInteger("Atk") == 0)
        {
            playerMove.FinishedMovement = canAttack;
        }

        if (!(
            ExecuteAttackIfPossible(KeyCode.Alpha1) ||
            ExecuteAttackIfPossible(KeyCode.Alpha2) ||
            ExecuteAttackIfPossible(KeyCode.Alpha3) ||
            ExecuteAttackIfPossible(KeyCode.Alpha4) ||
            ExecuteAttackIfPossible(KeyCode.Alpha5) ||
            ExecuteAttackIfPossible(KeyCode.Alpha6) ||
            ExecuteAttackIfPossible(Input.GetMouseButtonDown(1), KeyCode.Alpha6) 
        ))
        {
            animator.SetInteger("Atk", 0);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 targetPos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPos - transform.position),
                15f * Time.deltaTime);
        }
    }

    bool FadeAndWait(Image fadeImg, float fadeTime)
    {
        bool faded = false;

        if (null == fadeImg)
        {
            return faded;
        }

        if (!fadeImg.gameObject.activeInHierarchy)
        {
            fadeImg.gameObject.SetActive(true);
            fadeImg.fillAmount = 1f;
        }

        fadeImg.fillAmount -= fadeTime * Time.deltaTime;

        if (fadeImg.fillAmount <= 0.0f) {
            fadeImg.gameObject.SetActive(false);
            faded = true;
        }

        return faded;
    }

    bool ExecuteAttackIfPossible(KeyCode keyCode)
    {
        return ExecuteAttackIfPossible(Input.GetKeyDown(keyCode), keyCode);
    }

    bool ExecuteAttackIfPossible(bool keyDown, KeyCode keyCode)
    {
        int number = keyCode - KeyCode.Alpha1;
        if (keyDown)
        {
            playerMove.TargetPosition = transform.position;

            if (playerMove.FinishedMovement && !fadeImages[number] && canAttack)
            {
                animator.SetInteger("Atk", number + 1);
                fadeImages[number] = true;
                return true;
            }
        }

        return false;
    }
}
