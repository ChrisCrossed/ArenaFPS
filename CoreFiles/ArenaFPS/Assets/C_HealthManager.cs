using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;

public class C_HealthManager : MonoBehaviour
{
    // Player & Team Connections
    C_PlayerController PlayerController;
    PlayerIndex PlayerIndex;
    TeamColor TeamColor;

    // HUD Connections
    GameObject go_HUD;
    GameObject go_Healthbar;
    Image HealthBar_Left;
    Image HealthBar_Center;
    Image HealthBar_Right;

    // Use this for initialization
    void Start ()
    {
        SetPlayerIndex();
	}

    void SetPlayerIndex()
    {
        // Set Player Index
        PlayerController = gameObject.GetComponent<C_PlayerController>();
        PlayerIndex = PlayerController.player;
        TeamColor = PlayerController.TeamColor;

        // HUD Connections
        if (PlayerIndex == PlayerIndex.One)
            go_HUD = GameObject.Find("HUD_PlayerOne");
        else if (PlayerIndex == PlayerIndex.Two)
            go_HUD = GameObject.Find("HUD_PlayerTwo");
        else if (PlayerIndex == PlayerIndex.Three)
            { print("PLAYER THREE HUD NOT FOUND IN HEALTH MANAGER"); return; }
        else if (PlayerIndex == PlayerIndex.Four)
            { print("PLAYER FOUR HUD NOT FOUND IN HEALTH MANAGER"); return; }

        // Health Bar Connections
        go_Healthbar = go_HUD.transform.Find("Healthbar").gameObject;
        HealthBar_Left = go_Healthbar.transform.Find("HP_1").GetComponent<Image>();
        HealthBar_Center = go_Healthbar.transform.Find("HP_2").GetComponent<Image>();
        HealthBar_Right = go_Healthbar.transform.Find("HP_3").GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
