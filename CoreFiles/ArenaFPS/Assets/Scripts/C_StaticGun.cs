using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_StaticGun : C_WEAPON
{
    #region PATCH DATA
    // Damage done to an opponent, per second
    float DamagePerSecond;

    // Reduce opponent move speed
    float SlowPercent;

    // Rate to remove energy from an opponent
    float JetpackDrainRate;
    #endregion
    
    public void SET_DATA(STATICGUN_PATCH_DATA staticgunData_)
    {
        DamagePerSecond = staticgunData_.DamagePerSecond;
        SlowPercent = staticgunData_.SlowPercent;
        JetpackDrainRate = staticgunData_.JetpackDrainRate;
        MaxEnergyInSeconds = staticgunData_.MaxEnergyInSeconds;
        ReloadTimer_Max = staticgunData_.ReloadTimer_Max;
        
        InitializeWeapon();
    }

    // Use this for initialization
    void InitializeWeapon()
    {
        // string shotgunPelletFile = "Weapons/ShotgunPellet";
        // go_Bullet = (GameObject)Resources.Load(shotgunPelletFile, typeof(GameObject));

        CurrentEnergyInSeconds = MaxEnergyInSeconds;

        // Connect to pivot for reloading
        go_WeaponModel = transform.Find("mdl_Static").gameObject;
        go_PivotBall = go_WeaponModel.transform.Find("Cone").gameObject;
    }

    public void FireStaticGun()
    {
        // Don't let the gun fire if it's not ready
        if (WeaponState != WeaponState.Ready) return;

        if(CurrentEnergyInSeconds > 0f)
        {
            print("Bang");

            // Reduce energy amount
            CurrentEnergyInSeconds -= Time.deltaTime;

            if(CurrentEnergyInSeconds <= 0f)
            {
                print("Reloading!!");
                ReloadEnergy();
            }
        }
    }

    float StartingRotation;
    public override void Reload()
    {
        Vector3 v3_WeaponRot = go_PivotBall.transform.localEulerAngles;

        // First pass
        if (f_ReloadTimer == ReloadTimer_Max)
        {
            // Store starting rotation
            StartingRotation = v3_WeaponRot.y;
            print("Starting Rot: " + StartingRotation);

            // Subtract 360 degrees for rotation.
            v3_WeaponRot.y -= 360f;
        }

        // Done because parent is 'virtual public void Reload()'
        base.Reload();

        if(f_ReloadTimer >= 0f)
        {
            float f_Perc = f_ReloadTimer / ReloadTimer_Max;
            v3_WeaponRot.y = (f_Perc * 360f) + StartingRotation;

            if(f_ReloadTimer <= 0f)
            {
                v3_WeaponRot.y = StartingRotation;
            }
        }

        go_PivotBall.transform.localEulerAngles = v3_WeaponRot;
    }
}
