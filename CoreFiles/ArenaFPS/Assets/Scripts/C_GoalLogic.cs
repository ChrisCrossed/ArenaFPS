using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder2.Common;

public enum TeamColor
{
    Red,
    Blue
}

public class C_GoalLogic : MonoBehaviour
{
    [SerializeField] TeamColor GoalColor = TeamColor.Red;

    // SystemManager Connection
    GameObject go_SystemManager;
    C_SystemManager SystemManager;

    // Object Connections
    // GameObject go_BallSpawn;
    // C_BallLogic BallLogic;
    GameObject playerBlocker;

    // Receive Values for Entry Goal vs. Shot Goal

    // Use this for initialization
    void Start ()
    {
        if(GameObject.Find("SystemManager"))
        {
            go_SystemManager = GameObject.Find("SystemManager");
            SystemManager = go_SystemManager.GetComponent<C_SystemManager>();
        }

        if(GameObject.Find("BallSpawn"))
        {
            // go_BallSpawn = GameObject.Find("BallSpawn");
            // BallLogic = go_BallSpawn.GetComponent<C_BallLogic>();
        }

        if (transform.Find("Blocker"))
        {
            playerBlocker = transform.Find("Blocker").gameObject;

            // Set layer
            playerBlocker.layer = LayerMask.NameToLayer("BlockerRed");
            if (GoalColor == TeamColor.Blue) playerBlocker.layer = LayerMask.NameToLayer("BlockerBlue");
        }
    }

    public TeamColor GetGoalColor
    {
        get { return GoalColor; }
    }

    public void Score(int i_ScoreValue_, TeamColor teamColor_)
    {
        if(SystemManager)
        {
            SystemManager.Score(i_ScoreValue_, teamColor_);
        }
    }
    public void Score(int i_ScoreValue_)
    {
        TeamColor oppositeTeamColor = TeamColor.Red;
        if (GoalColor == TeamColor.Red) GoalColor = TeamColor.Blue;

        Score(i_ScoreValue_, oppositeTeamColor);
    }
}