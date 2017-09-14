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
    // TeamColor TeamColor;

    // HUD Connections
    GameObject go_HUD;
    GameObject go_Healthbar;
    Image HealthBar_Left;
    GameObject go_HealthBar_Left;
    Image HealthBar_Center;
    GameObject go_HealthBar_Center;
    Image HealthBar_Right;
    GameObject go_HealthBar_Right;
    Image ArmorBar;
    GameObject go_ArmorBar;

    // Use this for initialization
    void Awake ()
    {
        SetPlayerIndex();
	}

    void SetPlayerIndex()
    {
        // Set Player Index
        PlayerController = gameObject.GetComponent<C_PlayerController>();
        PlayerIndex = PlayerController.player;
        // TeamColor = PlayerController.TeamColor;

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
        go_Healthbar = go_HUD.transform.Find("HealthBar").gameObject;

        go_HealthBar_Left = go_Healthbar.transform.Find("HP_1").gameObject;
        HealthBar_Left = go_HealthBar_Left.GetComponent<Image>();

        go_HealthBar_Center = go_Healthbar.transform.Find("HP_2").gameObject;
        HealthBar_Center = go_HealthBar_Center.GetComponent<Image>();

        go_HealthBar_Right = go_Healthbar.transform.Find("HP_3").gameObject;
        HealthBar_Right = go_HealthBar_Right.GetComponent<Image>();

        go_ArmorBar = go_Healthbar.transform.Find("ArmorBar").gameObject;
        ArmorBar = go_ArmorBar.GetComponent<Image>();
    }

    public void SetHealthBar(int currHealth_, int maxHealth_)
    {
        SetHealthBar( (float)currHealth_ / (float)maxHealth_ );
    }
    public void SetHealthBar(float percent_)
    {
        // Healthbar One
        if (percent_ > 0.33f)
            HealthBar_Left.fillAmount = 1.0f;
        else
        {
            float f_Perc = percent_ / 0.33f;
            HealthBar_Left.fillAmount = f_Perc;
        }

        // Healthbar Two
        if (percent_ > 0.66f)
            HealthBar_Center.fillAmount = 1.0f;
        else if (percent_ > 0.33f)
        {
            float f_Perc = percent_ - 0.33f;
            f_Perc /= 0.33f;
            HealthBar_Center.fillAmount = f_Perc;
        }
        else HealthBar_Center.fillAmount = 0f;

        if (percent_ == 1.0f)
            HealthBar_Right.fillAmount = 1.0f;
        else if (percent_ > 0.66f)
        {
            float f_Perc = percent_ -= .66f;
            f_Perc /= 0.34f;
            HealthBar_Right.fillAmount = f_Perc;
        }
        else HealthBar_Right.fillAmount = 0f;
    }

    public void SetArmorBar(int currArmor_, int maxArmor_)
    {
        SetArmorBar( (float)currArmor_ / (float)maxArmor_);
    }
    public void SetArmorBar(float percent_)
    {
        ArmorBar.fillAmount = percent_;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
