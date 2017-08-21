using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponList
{
    None,
    Shotgun,
    MachineGun,
}

public class C_WEAPONMANAGER : C_INPUT_MANAGER
{
    // Bullet Container
    Transform t_BulletContainer;

    // Set Default weapon
    [SerializeField] WeaponList CurrentWeapon = WeaponList.Shotgun;
    [SerializeField] WeaponList PreviousWeapon = WeaponList.None;

    // Weapons Parent Object
    Transform this_WeaponObject;

    // Determine Team Color
    TeamColor teamColor;

    // Use this for initialization
    new void Start()
    {
        base.Start();

        // Bullet Container
        if(!GameObject.Find("Bullet Container"))
        {
            GameObject bulletContainer = new GameObject();
            bulletContainer.name = "Bullet Container";
            bulletContainer.transform.parent = null;
        }

        // Team Color
        teamColor = gameObject.GetComponent<C_PlayerController>().TeamColor;

        // Weapons Parent Object
        this_WeaponObject = transform.Find("Camera").transform.Find("Weapons");

        // Connections
        shotgun = this_WeaponObject.GetComponent<C_Shotgun>();
    }

    void QuickswitchWeapons()
    {
        WeaponList tempWeapon = PreviousWeapon;
        PreviousWeapon = CurrentWeapon;
        CurrentWeapon = tempWeapon;
    }

    public WeaponList Weapon
    {
        set
        {
            PreviousWeapon = CurrentWeapon;
            CurrentWeapon = value;
        }
        get { return PreviousWeapon; }
    }

    C_Shotgun shotgun;
    public void FireGun()
    {
        if(CurrentWeapon == WeaponList.Shotgun)
        {
            shotgun.FireShotgun(teamColor);
        }
        else if (CurrentWeapon == WeaponList.MachineGun)
        {

        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        base.UpdatePlayerInput();

        if(playerInput.Button_Y == XInputDotNetPure.ButtonState.Pressed)
        {
            print("Weapon Wheel");
        }

        // base.Update();

		// User holds down Left Bumper or 'Y' button

        // User uses Left or Right Analog stick, depending on button held, to select weapon

	}
}