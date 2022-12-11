using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private static volatile ScoreController instance = null;
    public delegate void GameWinHandler(int value);
    public event GameWinHandler GameWin;
    public int score;
    public const int winningPoint = 1;

    private readonly object scoreLock = new();

    private ScoreController() { }
    public static ScoreController Instance()
    {
        return instance;
    }
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetScore(0);
    }

    public void ObserveIsEscape(bool value)
    {
        int curScore = GetScore() + 1;
        SetScore(curScore);
        Debug.Log("the score is now:" + GetScore());
    }

    public int GetScore()
    {
        lock (scoreLock)
        {
            return score;
        }
    }
    private void SetScore(int value)
    {
        lock (scoreLock)
        {
            score = value;
            if (score >= winningPoint)
                GameWin?.Invoke(score);
        }
    }
}
