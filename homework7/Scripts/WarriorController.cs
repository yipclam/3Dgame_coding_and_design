using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    private Animator animator;
    float horizontal;
    float vertical;
    private bool isStay;
    private readonly object stayLock = new();

    bool isInvincible;
    float invincibleTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
    }

    void FixedUpdate()
    {
        if (isStay)
            return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        //Debug.Log("horizontal:"+horizontal+", vertical:"+vertical);
        ResetMovingAnime();
        if (horizontal < 0 && vertical > 0)
        {
            ForwardLeftAnime();
        }
        else if (horizontal == 0 && vertical > 0)
        {
            ForwardAnime();
        }
        else if (horizontal > 0 && vertical > 0)
        {
            ForwardRightAnime();
        }
        else if (horizontal > 0 && vertical == 0)
        {
            RightAnime();
        }
        else if (horizontal > 0 && vertical < 0)
        {
            BackwardRightAnime();
        }
        else if (horizontal == 0 && vertical < 0)
        {
            BackwardAnime();
        }
        else if (horizontal < 0 && vertical < 0)
        {
            BackwardLeftAnime();
        }
        else if (horizontal < 0 && vertical == 0)
        {
            LeftAnime();
        }
    }


    public void ForwardLeftAnime() { animator.SetBool("boolForwardLeftAnime", true); }
    public void ForwardAnime() { animator.SetBool("boolForwardAnime", true); }
    public void ForwardRightAnime() { animator.SetBool("boolForwardRightAnime", true); }
    public void RightAnime() { animator.SetBool("boolRightAnime", true); }
    public void BackwardRightAnime() { animator.SetBool("boolBackwardRightAnime", true); }
    public void BackwardAnime() { animator.SetBool("boolBackwardAnime", true); }
    public void BackwardLeftAnime() { animator.SetBool("boolBackwardLeftAnime", true); }
    public void LeftAnime() { animator.SetBool("boolLeftAnime", true); }

    public void ResetMovingAnime()
    {
        animator.SetBool("boolForwardLeftAnime", false);
        animator.SetBool("boolForwardAnime", false);
        animator.SetBool("boolForwardRightAnime", false);
        animator.SetBool("boolRightAnime", false);
        animator.SetBool("boolBackwardRightAnime", false);
        animator.SetBool("boolBackwardAnime", false);
        animator.SetBool("boolBackwardLeftAnime", false);
        animator.SetBool("boolLeftAnime", false);
    }

    public bool GetIsStay()
    {
        lock (stayLock)
        {
            return isStay;
        }
    }
    public void SetIsStay(bool isTrue)
    {
        lock (stayLock)
        {
            isStay = isTrue;
        }
    }
}
