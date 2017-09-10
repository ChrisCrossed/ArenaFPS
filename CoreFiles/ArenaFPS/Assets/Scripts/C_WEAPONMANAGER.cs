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

public class STATICGUN_PATCH_DATA
{
    // Performs 1% of health each 
    public float DamagePerSecond
    {
        get { return 1.05f; }
    }

    public float SlowPercent
    {
        get { return 0.75f; }
    }

    // Set to remove 60% of max energy over one full 'magazine'
    public float JetpackDrainRate
    {
        get { return 0.6f; }
    }

    // Length of time the gun can fire
    public float MaxEnergyInSeconds
    {
        get { return 5.0f; }
    }

    public float ReloadTimer_Max
    {
        get { return 1.0f; }
    }
}

public class C_WEAPONMANAGER : C_INPUT_MANAGER
{
    enum CurrentWeaponState
    {
        WeaponInUse,
        WaitingForResponse,
        ResponseReceived
    }

    CurrentWeaponState currentWeaponState = CurrentWeaponState.WeaponInUse;

    // Bullet Container
    Transform t_BulletContainer;

    // Set Default weapon
    [SerializeField] WeaponList CurrentWeapon = WeaponList.Shotgun;
    [SerializeField] WeaponList PreviousWeapon = WeaponList.None;

    // Weapons Parent Object
    Transform this_WeaponObject;

    // Weapon Connections
    C_Shotgun shotgun;
    C_StaticGun staticGun;

    // Determine Team Color
    TeamColor teamColor;

    #region Patch Data
    SHOTGUN_PATCH_DATA shotgunData;
    STATICGUN_PATCH_DATA staticgunData;
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
        staticGun = this_WeaponObject.GetComponent<C_StaticGun>();

        // Tell each weapon what their patch data is
        DispenseWeaponInformation();

        // Disable all weapons for initial run
        DisableAllPrimaryGuns();
    }

    void DispenseWeaponInformation()
    {
        // Create Weapon Data
        shotgunData = new SHOTGUN_PATCH_DATA();
        staticgunData = new STATICGUN_PATCH_DATA();

        // Dispense Data
        shotgun.SET_DATA(shotgunData);
        staticGun.SET_DATA(staticgunData);
    }

    void QuickswitchWeapons()
    {
        WeaponList tempWeapon = PreviousWeapon;
        PreviousWeapon = CurrentWeapon;
        CurrentWeapon = tempWeapon;
    }

    void DisableAllPrimaryGuns()
    {
        // Set 'Current Weapon' to none for startup
        CurrentWeapon = WeaponList.None;

        // Run through all weapons and disable them
        shotgun.MoveToInitialPosition();
        staticGun.MoveToInitialPosition();
        
        // Enable the shotgun
        SetNextGun(WeaponList.Shotgun);

        // Set CurrentWeaponState to 'waiting'
        ReadyForNextWeapon();
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
            staticGun.FireStaticGun();
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
            staticGun.ReloadEnergy();
        }
    }

    public void SetNextGun(WeaponList nextWeapon_)
    {
        // Don't continue if a gun is not already ready
        if (currentWeaponState != CurrentWeaponState.WeaponInUse)
        {
            print("RETURN - Not 'WeaponInUse': " + currentWeaponState.ToString());
            return;
        }

        // Don't run if they're the same weapon already
        if (nextWeapon_ == CurrentWeapon)
        {
            print("RETURN - Same Weapon': " + nextWeapon_.ToString());
            return;
        }

        // Tell current weapon to change state to 'Disable'
        if(CurrentWeapon == WeaponList.Shotgun)
        {
            shotgun.WeaponState = WeaponState.Disable;
        }
        else if(CurrentWeapon == WeaponList.StaticGun)
        {
            staticGun.WeaponState = WeaponState.Disable;
        }

        // Set Current Weapon
        PreviousWeapon = CurrentWeapon;
        CurrentWeapon = nextWeapon_;

        // Set CurrentWeaponState to 'waiting
        currentWeaponState = CurrentWeaponState.WeaponInUse;
    }

    public void ReadyForNextWeapon()
    {
        currentWeaponState = CurrentWeaponState.ResponseReceived;
    }

    private void Update()
    {
        if(currentWeaponState == CurrentWeaponState.ResponseReceived)
        {
            // Activate next weapon
            if(CurrentWeapon == WeaponList.Shotgun)
            {
                shotgun.WeaponState = WeaponState.Enable;
            }
            else if(CurrentWeapon == WeaponList.StaticGun)
            {
                staticGun.WeaponState = WeaponState.Enable;
            }

            currentWeaponState = CurrentWeaponState.WeaponInUse;
        }

        // TEST
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            ReadyForNextWeapon();
        }
    }
}