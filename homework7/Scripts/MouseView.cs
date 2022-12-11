using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseView : MonoBehaviour
{
    public double V = 5;
    private Vector2 mD;
    //The capsule parent!
    private Transform myBody;

    // Use this for initialization
    void Start()
    {
        myBody = this.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mC = new((float)(V*Input.GetAxisRaw("Mouse X")), (float)(V*Input.GetAxisRaw("Mouse Y")));

        mD += mC;
        //���¿������
        //��x����ת����ת��С
        this.transform.localRotation = Quaternion.AngleAxis(-mD.y, Vector3.right);
        //���ҿ��Ƹ���
        //��y����ת����ת��С
        myBody.localRotation = Quaternion.AngleAxis(mD.x, Vector3.up);
    }
}
