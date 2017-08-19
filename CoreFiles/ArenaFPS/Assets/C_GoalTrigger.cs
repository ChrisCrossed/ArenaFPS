using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GoalTrigger : MonoBehaviour
{
    // Parent Object
    Transform this_Parent;
    C_GoalLogic GoalLogic;

    // Trigger Info
    int i_LayerMask_Player;
    int i_LayerMask_Ball;

    // Use this for initialization
    void Start ()
    {
        i_LayerMask_Player = LayerMask.NameToLayer("Player");
        i_LayerMask_Ball = LayerMask.NameToLayer("Ball");

        // Parent Object
        this_Parent = transform.parent;
        GoalLogic = this_Parent.GetComponent<C_GoalLogic>();

        if (GoalLogic.GetGoalColor == TeamColor.Red) i_LayerMask_Player = LayerMask.NameToLayer("PlayerBlue");
        else i_LayerMask_Player = LayerMask.NameToLayer("PlayerRed");
    }

    private void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject.layer == i_LayerMask_Player)
        {
            GameObject go_Player = collider_.gameObject;
            C_PlayerController playerController = go_Player.GetComponent<C_PlayerController>();

            if (playerController.HasObjective)
            {
                // Determine what team to score for
                TeamColor oppositeTeamColor = TeamColor.Red;
                if (playerController.TeamColor == TeamColor.Red) oppositeTeamColor = TeamColor.Blue;

                // Remove objective from player inventory
                playerController.HasObjective = false;

                // Set score team earns for Entry Goal
                GoalLogic.Score(2, oppositeTeamColor);
            }
        }
        // Ball passes through
        else if(collider_.gameObject.layer == i_LayerMask_Ball)
        {
            GoalLogic.Score(1);
        }
    }
}
