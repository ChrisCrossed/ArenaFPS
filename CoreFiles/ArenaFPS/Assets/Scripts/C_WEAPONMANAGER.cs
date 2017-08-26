using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponList
{
    None,
    Shotgun,
    StaticGun,
}

public class SHOTGUN_PATCH_DATA
{
    // Each Pellet does this much damage on contact
    public int DamagePerPellet
    {
        get { return 3; }
    }

    // Number of pellets fired per shot
    public int NumberPelletsToFire
    {
        get { return 8; }
    }

    // Maximum random distance for pellet spread. Lower = more accurate.
    public float PelletSpread
    {
        get { return 3.5f; }
    }

    // Movement speed of the pellet
    public float PelletSpeed
    {
        get { return 3500f; }
    }

    // Number of shells before reloading
    public int i_ShotsInMagazine_Max
    {
        get { return 4; }
    }

    // Minimum time required between shots
    public float f_FireDelay_Max
    {
        get { return .5f; }
    }

    // Reload time
    public float ReloadTimer_Max
    {
        get { return 2f; }
    }
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

    // Weapon Connections
    C_Shotgun shotgun;

    // Determine Team Color
    TeamColor teamColor;

    #region Patch Data
    SHOTGUN_PATCH_DATA shotgunData;
    #endregion

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

        // Tell each weapon what their patch data is
        DispenseWeaponInformation();
    }

    void DispenseWeaponInformation()
    {
        // Create Weapon Data
        shotgunData = new SHOTGUN_PATCH_DATA();

        // Dispense Data
        shotgun.SET_DATA(shotgunData);
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
    
    public void FireGun()
    {
        if(CurrentWeapon == WeaponList.Shotgun)
        {
            shotgun.FireShotgun(teamColor);
        }
        else if (CurrentWeapon == WeaponList.StaticGun)
        {

        }
    }

    public void ReloadGun()
    {
        if (CurrentWeapon == WeaponList.Shotgun)
        {
            shotgun.ReloadGun();
        }
        else if (CurrentWeapon == WeaponList.StaticGun)
        {

        }
    }
}