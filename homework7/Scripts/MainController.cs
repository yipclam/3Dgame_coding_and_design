using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    private PatrolFactory patrolFactory;
    private ScoreController scoreController;
    private GameObject player;

    private bool isGameOver;
    private bool isGameWin;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;

        player = Instantiate(Resources.Load<GameObject>("Prefab/SwordWarrior"));
        player.transform.position = new Vector3(0, 0, -65);
        patrolFactory = PatrolFactory.Instance();
        patrolFactory.CreatePatrols();
        scoreController = ScoreController.Instance();
        scoreController.GameWin += ObserveGameWin;
        foreach(GameObject patrol in patrolFactory.GetPatrols())
        {
            patrol.GetComponent<PatrolController>().IsEscape += scoreController.ObserveIsEscape;
            patrol.GetComponent<PatrolController>().HasAttack += ObserveGameOver;
        } 
    }

    public int GetScore()
    {
        return scoreController.GetScore();
    }

    public void ObserveGameOver(bool value)
    {
        patrolFactory.StopPatrol();
        //player.GetComponent<PatrolController>().SetIsStay(true);
        Debug.Log("GameOver!");
        isGameOver = true;
    }

    public void ObserveGameWin(int value)
    {
        patrolFactory.StopPatrol();
        Debug.Log("you win! your score is now:" + value);
        isGameWin = true;
    }

    public void OnGUI()
    {
        if(isGameOver)
        {
            GUIStyle textStyle = new();
            textStyle.fontSize = 30;
            GUI.Label(new Rect(Screen.width / 2 - 55, Screen.width / 2 - 250, 100, 100), "游戏结束!", textStyle);
        }
        else if (isGameWin)
        {
            GUIStyle textStyle = new();
            textStyle.fontSize = 30;
            GUI.Label(new Rect(Screen.width / 2 - 55, Screen.width / 2 - 250, 100, 100), "游戏胜利!", textStyle);
        }
    }
}
