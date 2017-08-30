using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState
{
    Ready,
    Reloading,
    Enable,
    Disable
}

public class C_Shotgun : MonoBehaviour
{

    WeaponState weaponState = WeaponState.Ready;

    // Store player who fired
    Transform t_Player;

    #region PATCH DATA
    // Each Pellet does this much damage on contact
    int DamagePerPellet;

    // Number of pellets fired per shot
    int NumberPelletsToFire;

    // Maximum random distance for pellet spread. Lower = more accurate.
    float PelletSpread;

    // Movement speed of the pellet
    float PelletSpeed;

    // Number of shells before reloading
    int i_ShotsInMagazine_Max;

    // Minimum time required between shots
    float f_FireDelay_Max;

    // Reload time
    float ReloadTimer_Max;
    #endregion
    
    // Pellet connection
    GameObject go_Pellet;

    // Weapon Model
    GameObject go_ShotgunModel;

    // Weapon Manager
    C_WEAPONMANAGER WeaponManager;

    public void SET_DATA( SHOTGUN_PATCH_DATA shotgunData_ )
    {
        DamagePerPellet = shotgunData_.DamagePerPellet;
        NumberPelletsToFire = shotgunData_.NumberPelletsToFire;
        PelletSpread = shotgunData_.PelletSpread;
        PelletSpeed = shotgunData_.PelletSpeed;
        i_ShotsInMagazine_Max = shotgunData_.i_ShotsInMagazine_Max;
        f_FireDelay_Max = shotgunData_.f_FireDelay_Max;
        ReloadTimer_Max = shotgunData_.ReloadTimer_Max;

        InitializeWeapon();
    }

    public WeaponState WeaponState
    {
        set
        {
            if( weaponState != value)
            {
                weaponState = value;

                SetWeaponState(value);
            }
        }
        get
        {
            return weaponState;
        }
    }

    float f_WeaponState_Timer;
    float f_WeaponState_Timer_Max = 1.0f;
    float f_WeaponDisableRotation = 60f;
    void SetWeaponState( WeaponState weaponState_ )
    {
        if(weaponState_ == WeaponState.Enable)
        {
            f_WeaponState_Timer = f_WeaponState_Timer_Max;
        }
        else if(weaponState_ == WeaponState.Disable)
        {
            f_WeaponState_Timer = 0f;
        }
    }
    
    // Use this for initialization
    void InitializeWeapon()
    {
        string shotgunPelletFile = "Weapons/ShotgunPellet";
        go_Pellet = (GameObject)Resources.Load(shotgunPelletFile, typeof(GameObject));
        t_Player = transform.parent.transform.parent;
        WeaponManager = t_Player.GetComponent<C_WEAPONMANAGER>();

        i_ShotsInMagazine = i_ShotsInMagazine_Max;

        // Connect to pivot for reloading
        go_ShotgunModel = transform.Find("mdl_Shotgun").gameObject;
        go_PivotBall = go_ShotgunModel.transform.Find("PivotBall").gameObject;
    }

    float f_FireDelay;
    int i_ShotsInMagazine;
    public void FireShotgun(TeamColor teamColor_)
    {
        // Don't let the gun fire if it's not ready
        if (WeaponState != WeaponState.Ready) return;
        
        // Check 'magazine' count
        if (i_ShotsInMagazine > 0 && f_FireDelay == 0)
        {
            // Fire group of pellets
            for(int i = 0; i < NumberPelletsToFire; ++i)
            {
                GameObject go_Pellet_ = Instantiate(go_Pellet);

                Transform projectilePoint = transform.Find("mdl_Shotgun").transform.Find("ProjectilePoint");
                go_Pellet_.transform.position = projectilePoint.position;
                go_Pellet_.transform.rotation = projectilePoint.rotation;
                go_Pellet_.transform.SetParent(GameObject.Find("Bullet Container").transform);

                #region Set Bullet Color
                Material[] mat_ = go_Pellet_.GetComponent<TrailRenderer>().materials;
                if(teamColor_ == TeamColor.Red)
                {
                    for(int j = 0; j < mat_.Length; ++j)
                        mat_[j] = (Material)Resources.Load("Materials/Shotgun/mat_Red");
                }
                else
                {
                    for (int j = 0; j < mat_.Length; ++j)
                        mat_[j] = (Material)Resources.Load("Materials/Shotgun/mat_Blue");
                }
                go_Pellet_.GetComponent<TrailRenderer>().materials = mat_;
                #endregion

                go_Pellet_.GetComponent<C_ShotgunPellet>().SpawnBullet( PelletSpeed, t_Player.gameObject, projectilePoint.gameObject, teamColor_, DamagePerPellet, PelletSpread );
            }

            // Reset delay until next shot
            f_FireDelay = f_FireDelay_Max;

            // Reduce shot count
            --i_ShotsInMagazine;

            if(i_ShotsInMagazine == 0)
            {
                ReloadGun();
            }
        }
    }

    GameObject go_PivotBall;
    float f_ReloadTimer;
    void Reload()
    {
        // Reduce timer
        if(f_ReloadTimer > 0) 
        {
            f_ReloadTimer -= Time.deltaTime;

            if (f_ReloadTimer < 0)
            {
                f_ReloadTimer = 0f;
                WeaponState = WeaponState.Ready;
                i_ShotsInMagazine = i_ShotsInMagazine_Max;
            }
        }

        Vector3 v3_PivotBallRot = go_PivotBall.transform.localEulerAngles;

        float TimeToMoveToPosition = 0.5f;
        float GunDownTime = ReloadTimer_Max - TimeToMoveToPosition;
        float GunUpTime = TimeToMoveToPosition;

        // f_Timer counts downward from Max until 0.
        if (f_ReloadTimer > GunDownTime)
        {
            if (v3_PivotBallRot.x < 30f)
            {
                v3_PivotBallRot.x += Time.deltaTime * 30f * 2f;

                if (v3_PivotBallRot.x > 30f) v3_PivotBallRot.x = 30f;
                print(v3_PivotBallRot.x);
            }
        }
        else if(f_ReloadTimer < GunUpTime)
        {
            if(v3_PivotBallRot.x > 0f)
            {
                v3_PivotBallRot.x -= Time.deltaTime * 30f * 2f;

                if (f_ReloadTimer == 0f) v3_PivotBallRot.x = 0f;
                print(v3_PivotBallRot.x);
            }
        }

        // Apply rotation
        go_PivotBall.transform.localEulerAngles = v3_PivotBallRot;
    }

    public void ReloadGun()
    {
        if( i_ShotsInMagazine < i_ShotsInMagazine_Max && weaponState != WeaponState.Reloading )
        {
            WeaponState = WeaponState.Reloading;
            f_ReloadTimer = ReloadTimer_Max;
        }
    }
    
    // Update is called once per frame
    void Update ()
    {
        if(f_FireDelay > 0f)
        {
            f_FireDelay -= Time.deltaTime;
            if (f_FireDelay < 0f) f_FireDelay = 0f;
        }

		if(WeaponState == WeaponState.Reloading)
        {
            Reload();
        }

        if (WeaponState == WeaponState.Enable)
        {
            if (f_WeaponState_Timer > 0f)
            {
                f_WeaponState_Timer -= Time.deltaTime;

                if (f_WeaponState_Timer <= 0f)
                {
                    f_WeaponState_Timer = 0f;

                    WeaponState = WeaponState.Ready;
                }

                print(f_WeaponState_Timer);

                // Rotate game object based on percentage
                float f_Perc = f_WeaponState_Timer / f_WeaponState_Timer_Max;
                Vector3 v3_Rot = go_ShotgunModel.transform.localEulerAngles;
                v3_Rot.x = f_Perc * f_WeaponDisableRotation;
                go_ShotgunModel.transform.localEulerAngles = v3_Rot;
            }
        }
        else if (WeaponState == WeaponState.Disable)
        {
            if (f_WeaponState_Timer < f_WeaponState_Timer_Max)
            {
                // Begin adding to weaponstate timer.
                f_WeaponState_Timer += Time.deltaTime;

                // If capped out, set state to ready.
                if (f_WeaponState_Timer > f_WeaponState_Timer_Max)
                {
                    f_WeaponState_Timer = f_WeaponState_Timer_Max;
                    
                    // Tell Weapon Manager we completed
                    WeaponManager.ReadyForNextWeapon();
                }

                // Rotate game object based on percentage.
                float f_Perc = f_WeaponState_Timer / f_WeaponState_Timer_Max;
                Vector3 v3_Rot = go_ShotgunModel.transform.localEulerAngles;
                v3_Rot.x = f_Perc * f_WeaponDisableRotation;
                go_ShotgunModel.transform.localEulerAngles = v3_Rot;
            }

            
        }
	}
}
