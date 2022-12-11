using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PatrolController : MonoBehaviour
{
    private Animator animator;
    private GameObject target;

    public float radiusOfPatrol;
    public float fieldOfView;//视野度数
    public Vector3 centerPos;
    Vector3 nextPos;
    public delegate void isEscapeHandler(bool value);
    public event isEscapeHandler IsEscape;
    public delegate void HasAttackHandler(bool value);
    public event HasAttackHandler HasAttack;

    public bool isPatrolling;
    private bool isArrived;
    private bool isInView;
    private bool isCanSee;
    private bool isStay;
    private readonly object nextPosLock = new();
    private readonly object arrivedLock = new();
    private readonly object patrollLock = new();
    private readonly object inViewLock = new();
    private readonly object canSeeLock = new();
    private readonly object stayLock = new();

    // Start is called before the first frame update
    void Start()
    {
        fieldOfView = 150.0f;

        animator = GetComponent<Animator>();
        nextPos = new Vector3();

        SetIsPatrolling(true);
        SetIsArrived(true);
        SetIsInView(false);
        SetIsCanSee(false);
        SetIsStay(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);//必须放在update里
        ResetAnime();
        if (isStay)
            return;
        if(Engage())
        {
            SetIsPatrolling(false);
            SetIsArrived(false);
            GoToPosition(target.transform.localPosition);
            if (IsCloseToTarget())
            {
                TurnToPosition(target.transform.localPosition);
                AttackAnime();
                if (animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("RightHand@Attack01"))
                {
                    SetIsPatrolling(true);
                    SetIsArrived(true);
                    Debug.Log("enenmy hitted!");
                    HasAttack?.Invoke(true);
                }
            }   
        }
        if(!Engage() && !GetIsPatrolling())
        {
            SetIsPatrolling(true);
            Debug.Log("Escape!");
            IsEscape?.Invoke(true);
        }
        if (GetIsPatrolling())
        {
            Patrol();
        }
    }

    
    void Patrol()
    {
        if(IsInArea() && GetIsArrived())
        {
            lock(nextPosLock)
            {
                nextPos = GetNextPos();
            }
            SetIsArrived(false);
        }
        else if(!IsInArea() && GetIsArrived())
        {
            lock (nextPosLock)
            {
                System.Random rd = new();
                float homePoint = rd.Next(-1, 2);
                //Debug.Log("homePoint: " + homePoint);
                nextPos.x = centerPos.x + homePoint * radiusOfPatrol;
                nextPos.y = this.transform.localPosition.y;
                nextPos.z = centerPos.z;
            }
            SetIsArrived(false);
        }
        lock (nextPosLock)
        {
            GoToPosition(nextPos);
        }  
    }
    bool IsInArea()
    {
        Vector3 curPos = this.transform.localPosition;
        double distance = Math.Pow(Math.Abs(curPos.x - centerPos.z), 2.0) + Math.Pow(Math.Abs(curPos.z - centerPos.z), 2.0);
        return Math.Abs(distance - Math.Pow(radiusOfPatrol, 2.0)) < 0.8;
    }
    Vector3 GetNextPos()
    {
        nextPos = new Vector3();
        System.Random rd = new();
        float nextAngle = rd.Next(1, 180);
        Vector3 curPos = this.transform.localPosition;
        float cosA = (curPos.x - centerPos.x) / radiusOfPatrol;
        if (Math.Abs(cosA) > 1)
            cosA = 1;
        float angleA = MathF.Acos(cosA) * (180/MathF.PI);
        float angleC = angleA + nextAngle;
        //Debug.Log(cosA +" "+ angleA +" "+ angleC);

        nextPos.x = radiusOfPatrol * MathF.Cos(angleC) + centerPos.x;
        nextPos.y = curPos.y;
        nextPos.z = radiusOfPatrol * MathF.Sin(angleC) + centerPos.z;
        Debug.Log("getNextPos: " + nextPos.x + " " + nextPos.z);
        return nextPos;
    }
    void GoToPosition(Vector3 targetPos)
    {
        /*var direction = targetPos - this.transform.localPosition;//目标方向
        transform.Translate(direction.normalized * Time.deltaTime * 0.5f, Space.World);//向目标方向移动，normalized归一实现匀速移动*/
        float disX = MathF.Abs(targetPos.x - this.transform.localPosition.x);
        float disZ = MathF.Abs(targetPos.z - this.transform.localPosition.z);
        if (disX < 0.2 && disZ < 0.2)
        {
            //Debug.Log("isArrived!, " + this.transform.localPosition.x + " " + this.transform.localPosition.z + " " + disX + " " + disZ);
            SetIsArrived(true);
        }
        else if(disX < 1.0 && disZ < 1.0)
        {
            var direction = targetPos - this.transform.localPosition;//目标方向
            transform.Translate(1.0f * Time.deltaTime * direction.normalized, Space.World);//向目标方向移动，normalized归一实现匀速移动
            //Debug.Log("close to arrive: " + targetPos.x + " " + targetPos.z);
        }
        else
        {
            TurnToPosition(targetPos);
            MoveAnime();
            //Debug.Log("is moving to: " + targetPos.x + " " + targetPos.z);
        }
    }
    void TurnToPosition(Vector3 targetPos)
    {
        var direction = targetPos - this.transform.localPosition;//目标方向
        var angle = Vector3.Angle(transform.forward, direction);//获取夹角
        var cross = Vector3.Cross(transform.forward, direction);

        var turn = cross.y >= 0 ? 1f : -1f;
        transform.Rotate(transform.up, angle * Time.deltaTime * 5f * turn, Space.World);
    }


    bool Engage()
    {
        return GetIsInView() && GetIsCanSee();
    }
    bool RayCheck()
    {
        Vector3 forward = transform.forward;//人物前方正方向
        Vector3 playerDir = target.transform.position - transform.position;//人物到被检测物体的方向
        float temp = Vector3.Angle(forward, playerDir);//求出角度
        //向被检测物体发射射线，为了判断之间是否有障碍物遮挡
        bool res = Physics.Raycast(transform.position + Vector3.up, target.transform.position - transform.position, out RaycastHit hitInfo);
        //Debug.Log("rayCheck: " + temp + " " + res + " " + hitInfo.transform.name);
        if (temp < 0.5f * fieldOfView && (res == false || hitInfo.collider.CompareTag("Player")))
        {
            return true;//被检测物体在视野中
        }
        return false;//被检测物体不在视野中
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject;
            //提前计算角度差
            SetIsInView(true);
            SetIsCanSee(RayCheck());
            //Debug.Log("Enter: " + GetIsInView() + " " + GetIsCanSee());
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (target == null)
            {
                target = other.gameObject;
            }
            //SetIsInView(true);
            //SetIsCanSee(RayCheck());
            Debug.Log("Stay: " + GetIsInView() + " " + GetIsCanSee());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
            SetIsInView(false);
            SetIsCanSee(false);
        }
    }
    bool IsCloseToTarget()
    {
        Vector3 curPos = this.transform.localPosition;
        double distance = Math.Pow((curPos.x - target.transform.localPosition.x), 2.0) + Math.Pow((curPos.z - target.transform.localPosition.z), 2.0);
        return Math.Abs(distance) < 9;
    }


    public void MoveAnime() { animator.SetBool("boolMoveAnime", true); }
    public void AttackAnime() { animator.SetBool("boolAttackAnime", true); }
    public void StayAnime() { animator.SetBool("boolStayAnime", true); }
    public void ResetAnime()
    {
        animator.SetBool("boolMoveAnime", false);
        animator.SetBool("boolAttackAnime", false);
        //animator.SetBool("boolStayAnime", false);
    }


    private bool GetIsArrived()
    {
        lock (arrivedLock)
        {
            return isArrived;
        }
    }
    private void SetIsArrived(bool isTrue)
    {
        lock(arrivedLock)
        {
            isArrived = isTrue;
        }
    }
    private bool GetIsPatrolling()
    {
        lock (patrollLock)
        {
            return isPatrolling;
        }
    }
    private void SetIsPatrolling(bool isTrue)
    {
        lock (patrollLock)
        {
            isPatrolling = isTrue;
        }
    }
    private bool GetIsInView()
    {
        lock (inViewLock)
        {
            return isInView;
        }
    }
    private void SetIsInView(bool isTrue)
    {
        lock (inViewLock)
        {
            isInView = isTrue;
        }
    }
    private bool GetIsCanSee()
    {
        lock (canSeeLock)
        {
            return isCanSee;
        }
    }
    private void SetIsCanSee(bool isTrue)
    {
        lock (canSeeLock)
        {
            isCanSee = isTrue;
        }
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
