using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponList
{
    None = -1,
    Shotgun = 0,
    StaticGun = 3,
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
    [SerializeField] WeaponList PrevWeapon = WeaponList.None;

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

        WeaponIconMaxAlpha = new float[8];
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
            if (WeaponImplementationTest(i)) continue;

            WeaponHUD_WeaponIcons[i] = WeaponHUD.transform.Find("Weapon_" + i).gameObject;
            WeaponIcons[i] = WeaponHUD_WeaponIcons[i].GetComponent<Image>();
        }

        // Turn off HUD Alpha's
        HUDBackgroundAlpha = 0f;

        // Turn off all selection icons
        Color clr_;
        for(int i_ = 0; i_ < 8; ++i_)
        {
            if (!(i_ == 0 || i_ == 3)) continue;

            clr_ = WeaponIcons[i_].color;
            clr_.a = 0f;
            WeaponIcons[i_].color = clr_;
        }

        // Set Max Weapon Icon Alpha
        
        
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

    public void DisableAllPrimaryGuns()
    {
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

    public bool WeaponHUDState
    {
        set;
        get;
    }

    float TransparencyHalf = 0.3f;
    float TransparencyOff = 1.0f;
    float[] WeaponIconMaxAlpha;
    public void WeaponHUDSetWeaponActive(int weaponNumber_, bool setActive_)
    {
        // TEMPORARY
        if (WeaponImplementationTest(weaponNumber_)) return;

        // If the player owns the weapon, the icon has no transparency
        if (setActive_)
            WeaponIconMaxAlpha[weaponNumber_] = TransparencyOff;
        else // The weapon isn't owned and is heavily transparent
            WeaponIconMaxAlpha[weaponNumber_] = TransparencyHalf;
    }

    float FadeOutTimer = 0.0f;
    float FadeOutTimerMax = 2f;
    public void WeaponHUDChoice(int weaponNumber_)
    {
        Color clr_;
        for(int i_ = 0; i_ < 8; ++i_)
        {
            // Record state of icon color
            clr_ = SelectIcons[i_].color;

            // Don't try and change the alpha of unneeded icons.
            if ((clr_.a == 0f && i_ != weaponNumber_)) continue; // If this icon doesn't need to be changed, skip it.

            // Selection made, reset Fade Out Timer
            FadeOutTimer = FadeOutTimerMax;

            // Increase or Decrease transparency
            if (i_ == weaponNumber_) clr_.a = 1f;
            else clr_.a -= Time.deltaTime * 10f;

            // Cap
            if (clr_.a < 0) clr_.a = 0f;

            // Set
            SelectIcons[i_].color = clr_;
        }
    }

    float AlphaLerp;
    public void ShowWeaponIconAlpha(bool IsActive_ = false)
    {
        // Increase or decrease alpha
        if (IsActive_ && AlphaLerp < 1.0f)
        {
            AlphaLerp += Time.deltaTime * 10f;

            if (AlphaLerp > 1.0f) AlphaLerp = 1.0f;
        }
        else if (!IsActive_ && AlphaLerp > 0f)
        {
            AlphaLerp -= Time.deltaTime * 10f;

            if (AlphaLerp < 0f) AlphaLerp = 0f;
        }
        else return;

        // Set each weapon icon alpha
        Color clr_;
        for(int i_ = 0; i_ < 8; ++i_)
        {
            if (WeaponImplementationTest(i_)) continue;

            clr_ = WeaponIcons[i_].color;
            clr_.a = AlphaLerp * WeaponIconMaxAlpha[i_];
            WeaponIcons[i_].color = clr_;
        }
    }

    // Sets the current HUD background alpha channel between 0f and 1f.
    float BackgroundAlpha;
    float HUDBackgroundAlpha
    {
        // Sets the Background alpha
        set
        {
            BackgroundAlpha = value;

            Color clr_ = Background.color;
            clr_.a = BackgroundAlpha;
            Background.color = clr_;
        }
        // Returns the Background Alpha
        get
        {
            return BackgroundAlpha;
        }
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

        // Show Weapon Icons
        ShowWeaponIconAlpha(WeaponHUDState);

        // Weapon HUD Wheel
        if (WeaponHUDState)
        {
            #region Set HUD Alpha
            // If HUD is less than 1.0f, increase
            if(HUDBackgroundAlpha < 1.0f)
            {
                // Temporary float so we're not changing the object a bunch
                float temp_ = HUDBackgroundAlpha;

                // Fade in HUD
                temp_ += Time.deltaTime * 10f;

                // Cap
                if (temp_ > 1.0f) temp_ = 1.0f;

                // Assign
                HUDBackgroundAlpha = temp_;
            }
            #endregion
        }
        else
        {
            if(HUDBackgroundAlpha > 0f)
            {
                // Temporary float so we're not changing the object a bunch
                float temp_ = HUDBackgroundAlpha;

                // Fade out HUD
                temp_ -= Time.deltaTime * 10f;

                // Cap
                if (temp_ < 0f) temp_ = 0f;

                // Assign
                HUDBackgroundAlpha = temp_;
            }
        }
    }

    bool WeaponImplementationTest(int i_)
    {
        bool CanContinue = false;

        if (i_ == 0) CanContinue = true;
        else if (i_ == 3) CanContinue = true;

        return !CanContinue;
    }
}