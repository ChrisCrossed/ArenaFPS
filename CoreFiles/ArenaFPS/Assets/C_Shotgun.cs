using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Shotgun : MonoBehaviour
{
    enum WeaponState
    {
        Ready,
        Reloading
    }

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
        go_Pellet = (GameObject)Resources.Load(shotgunPelletFile, typeof(GameObject));
        t_Player = transform.parent.transform.parent;

        i_ShotsInMagazine = i_ShotsInMagazine_Max;

        // Connect to pivot for reloading
        go_PivotBall = transform.Find("mdl_Shotgun").Find("PivotBall").gameObject;
    }

    float f_FireDelay;
    int i_ShotsInMagazine;
    public void FireShotgun(TeamColor teamColor_)
    {
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
        if(f_ReloadTimer > 0) f_ReloadTimer -= Time.deltaTime;
        {
            if (f_ReloadTimer < 0)
            {
                f_ReloadTimer = 0f;
                weaponState = WeaponState.Ready;
                i_ShotsInMagazine = i_ShotsInMagazine_Max;
            }
        }

        Vector3 v3_PivotBallRot = go_PivotBall.transform.localEulerAngles;

        // Put barrel down to reload
        if(f_ReloadTimer > Time.deltaTime * 75f)
        {
            if(v3_PivotBallRot.x < 30f)
            {
                v3_PivotBallRot.x += Time.deltaTime * 75f;

                if (v3_PivotBallRot.x > 30f) v3_PivotBallRot.x = 30f;
            }
        }
        else
        {
            if(v3_PivotBallRot.x > 0f)
            {
                v3_PivotBallRot.x -= Time.deltaTime * 75f;

                if (v3_PivotBallRot.x < 0) v3_PivotBallRot.x = 0f;
            }
        }

        // We already confirmed above that the weapon is done reloading. Snap to proper position.
        if (weaponState == WeaponState.Ready) v3_PivotBallRot.x = 0f;

        go_PivotBall.transform.localEulerAngles = v3_PivotBallRot;
    }

    public void ReloadGun()
    {
        if( i_ShotsInMagazine < i_ShotsInMagazine_Max && weaponState != WeaponState.Reloading )
        {
            weaponState = WeaponState.Reloading;
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

		if(weaponState == WeaponState.Reloading)
        {
            Reload();
        }
	}
}
