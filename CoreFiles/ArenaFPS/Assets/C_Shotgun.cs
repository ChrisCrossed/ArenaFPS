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

public class C_Shotgun : C_WEAPON
{
    #region PATCH DATA
    // Each Pellet does this much damage on contact
    int DamagePerPellet;

    // Number of pellets fired per shot
    int NumberPelletsToFire;

    // Maximum random distance for pellet spread. Lower = more accurate.
    float PelletSpread;

    // Movement speed of the pellet
    float PelletSpeed;
    
    // Minimum time required between shots
    float f_FireDelay_Max;

    // Reload time
    float ReloadTimer_Max;
    #endregion
    
    // Pellet connection
    GameObject go_Bullet;

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
    
    // Use this for initialization
    void InitializeWeapon()
    {
        string shotgunPelletFile = "Weapons/ShotgunPellet";
        go_Bullet = (GameObject)Resources.Load(shotgunPelletFile, typeof(GameObject));
        
        i_ShotsInMagazine = i_ShotsInMagazine_Max;

        // Connect to pivot for reloading
        go_WeaponModel = transform.Find("mdl_Shotgun").gameObject;
        go_PivotBall = go_WeaponModel.transform.Find("PivotBall").gameObject;
    }
    
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
                GameObject go_Pellet_ = Instantiate(go_Bullet);

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
    
    public override void Reload()
    {
        // Done because parent is 'virtual public void Reload()'
        base.Reload();

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
            }
        }
        else if(f_ReloadTimer < GunUpTime)
        {
            if(v3_PivotBallRot.x > 0f)
            {
                v3_PivotBallRot.x -= Time.deltaTime * 30f * 2f;

                if (f_ReloadTimer == 0f) v3_PivotBallRot.x = 0f;
            }
        }

        // Apply rotation
        go_PivotBall.transform.localEulerAngles = v3_PivotBallRot;
    }

    public void ReloadGun()
    {
        if( i_ShotsInMagazine < i_ShotsInMagazine_Max && WeaponState != WeaponState.Reloading )
        {
            WeaponState = WeaponState.Reloading;
            f_ReloadTimer = ReloadTimer_Max;
        }
    }
}
