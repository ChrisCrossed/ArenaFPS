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
    [SerializeField] WeaponList PrevWeapon;

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

    // Weapon HUD
    GameObject WeaponHUD;
    GameObject WeaponHUD_Background;
    GameObject[] WeaponHUD_SelectIcons;
    GameObject[] WeaponHUD_WeaponIcons;
    Image Background;
    Image[] SelectIcons;
    Image[] WeaponIcons;

    private void Awake()
    {
        // Bullet Container
        if (!GameObject.Find("Bullet Container"))
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
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();

        // Tell each weapon what their patch data is
        DispenseWeaponInformation();

        // Disable all weapons for initial run
        DisableAllPrimaryGuns();

        // Set up HUD Icons
        InitializeHUD();
    }

    GameObject PlayerHUD;
    void InitializeHUD()
    {
        // Set correct HUD
        if (player == XInputDotNetPure.PlayerIndex.One)
            PlayerHUD = GameObject.Find("HUD_PlayerOne");
        else if (player == XInputDotNetPure.PlayerIndex.Two)
            PlayerHUD = GameObject.Find("HUD_PlayerTwo");
        else if (player == XInputDotNetPure.PlayerIndex.Three)
            PlayerHUD = GameObject.Find("HUD_PlayerThree");
        else
            PlayerHUD = GameObject.Find("HUD_PlayerFour");

        // WeaponHUD Connections
        WeaponHUD = PlayerHUD.transform.Find("WeaponWheel").gameObject;
        WeaponHUD_SelectIcons = new GameObject[8];
        WeaponHUD_WeaponIcons = new GameObject[8];
        SelectIcons = new Image[WeaponHUD_SelectIcons.Length];
        WeaponIcons = new Image[WeaponHUD_WeaponIcons.Length];

        // Set WeaponHUD Connections
        WeaponHUD_Background = WeaponHUD.transform.Find("Background").gameObject;
        Background = WeaponHUD_Background.GetComponent<Image>();
        for (int i = 0; i < 8; ++i)
        {
            WeaponHUD_SelectIcons[i] = WeaponHUD.transform.Find("Select_" + i).gameObject;
            SelectIcons[i] = WeaponHUD_SelectIcons[i].GetComponent<Image>();

            // TEMPORARY - CHANGE WHEN ALL WEAPONS EXIST
            if (!(i == 0 || i == 3)) continue;

            WeaponHUD_WeaponIcons[i] = WeaponHUD.transform.Find("Weapon_" + i).gameObject;
            WeaponIcons[i] = WeaponHUD_WeaponIcons[i].GetComponent<Image>();
        }

        // Set current transparency
        SetHUDBackgroundTransparency(0.4f);
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

    public void QuickswitchWeapons()
    {
        // Move to new weapon
        SetNextGun(PreviousWeapon);
    }

    void DisableAllPrimaryGuns()
    {
        // Set 'Current Weapon' to none for startup
        // PreviousWeapon = WeaponList.None;

        // Run through all weapons and disable them
        shotgun.MoveToInitialPosition();
        staticGun.MoveToInitialPosition();

        // Set CurrentWeaponState to 'waiting'
        ReadyForNextWeapon();
    }

    public WeaponList Weapon
    {
        set
        {
            PrevWeapon = CurrentWeapon;
            CurrentWeapon = value;
        }
        get { return CurrentWeapon; }
    }
    public WeaponList PreviousWeapon
    {
        get { return PrevWeapon; }
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
        PrevWeapon = CurrentWeapon;
        CurrentWeapon = nextWeapon_;

        // Set CurrentWeaponState to 'waiting
        currentWeaponState = CurrentWeaponState.WeaponInUse;
    }

    public void ReadyForNextWeapon()
    {
        currentWeaponState = CurrentWeaponState.ResponseReceived;
    }

    float TransparencyHalf = 0.5f;
    float TransparencyOff = 1.0f;
    public bool WeaponHUDState
    {
        set;
        get;
    }

    public void WeaponHUDSetWeaponActive(int weaponNumber_, bool setActive_)
    {

    }

    public void WeaponHUDChoice(int weaponNumber_)
    {
        Color clr_;
        for(int i_ = 0; i_ < 8; ++i_)
        {
            clr_ = SelectIcons[i_].color;

            if (i_ == weaponNumber_) clr_.a += Time.deltaTime * 10f;
            else clr_.a -= Time.deltaTime * 10f;

            if (clr_.a > 1.0f) clr_.a = 1.0f;
            else if (clr_.a < 0) clr_.a = 0f;

            SelectIcons[i_].color = clr_;
        }
    }

    void SetHUDBackgroundTransparency(float transparency_)
    {
        Color clr_ = Background.color;
        clr_.a = transparency_;
        Background.color = clr_;
    }

    float f_TestTimer;
    int count;
    private void Update()
    {
        // Current Weapon Listener
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

        // Weapon HUD Wheel
        if(WeaponHUDState)
        {
            // Fade in HUD
            // GetWeaponHUD
        }

        f_TestTimer += Time.deltaTime;
        if (f_TestTimer > 0.15f)
        {
            f_TestTimer = 0f;

            ++count;
            if (count >= 10) count = 0;
        }

        WeaponHUDChoice(count);
    }
}