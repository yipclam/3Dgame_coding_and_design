using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFactory : MonoBehaviour
{
    private List<GameObject> usedPatrols = new();
    private Vector3[] vec = new Vector3[3];     // 保存每个巡逻兵的初始位置

    private static volatile PatrolFactory instance = null;//保证instance在所有线程中同步
                                                          //private防止类在外部被实例化
    private PatrolFactory(){ }
    public static PatrolFactory Instance()
    {
        return instance;
    }
    void Awake()
    {
        instance = this;
    }

    public List<GameObject> CreatePatrols()
    {
        int[] pos_x = { 0, 0, 0 };
        int[] pos_z = { 40, 0, -40 };
        int[] radiusOfPatrol = { 10, 10, 10 };
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            vec[index] = new Vector3(pos_x[i], 0, pos_z[i]);
            index++;
        }
        
        for (int i = 0; i < 3; i++)
        {
            GameObject patrol = Instantiate(Resources.Load<GameObject>("Prefab/ShieldWarrior"));
            patrol.transform.position = vec[i];
            patrol.GetComponent<PatrolController>().centerPos = vec[i];
            patrol.GetComponent<PatrolController>().radiusOfPatrol = radiusOfPatrol[i];

            usedPatrols.Add(patrol);
            
        }
        return usedPatrols;
    }

    public void StopPatrol()
    {
        for (int i = 0; i < usedPatrols.Count; i++)
        {
            usedPatrols[i].GetComponent<PatrolController>().SetIsStay(true);
        }
    }

    public List<GameObject> GetPatrols()
    {
        return usedPatrols;
    }

}
