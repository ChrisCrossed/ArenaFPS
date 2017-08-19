using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SystemManager : MonoBehaviour
{
    // Connect to Ball Object (If it exists)
    GameObject go_Ball;
    C_BallLogic ballLogic;

    // Team Scores
    int i_Score_Blue;
    int i_Score_Red;
    [SerializeField] int i_Score_Max = 3;

    // Use this for initialization
    void Start ()
    {
		if(GameObject.Find("BallSpawn"))
        {
            go_Ball = GameObject.Find("BallSpawn");
            ballLogic = go_Ball.GetComponent<C_BallLogic>();
        }
	}

    public void SetBallState(bool b_IsActive_)
    {
        if(ballLogic)
        {
            ballLogic.IsActive = b_IsActive_;
        }
    }

    public void Score(int i_Points_, TeamColor teamColor_)
    {
        SetBallState(true);

        #region Score Text on Console
        string ScoreOutput;

        // Print scoring team name
        if (teamColor_ == TeamColor.Red)
        {
            ScoreOutput = "Blue Team ";
        }
        else
        {
            ScoreOutput = "Red Team ";
        }

        // Apply score
        print(ScoreOutput + "Scored " + i_Points_ + " points");
        #endregion
    }
}