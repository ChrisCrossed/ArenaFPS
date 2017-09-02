using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_StaticGun : C_WEAPON
{
    public void SET_DATA(STATICGUN_PATCH_DATA staticgunData_)
    {
        /*
        DamagePerPellet = staticgunData_.DamagePerSecond;
        NumberPelletsToFire = staticgunData_.NumberPelletsToFire;
        PelletSpread = staticgunData_.PelletSpread;
        PelletSpeed = staticgunData_.PelletSpeed;
        i_ShotsInMagazine_Max = staticgunData_.i_ShotsInMagazine_Max;
        f_FireDelay_Max = staticgunData_.f_FireDelay_Max;
        ReloadTimer_Max = staticgunData_.ReloadTimer_Max;
        */

        InitializeWeapon();
    }

    // Use this for initialization
    void InitializeWeapon()
    {
        // string shotgunPelletFile = "Weapons/ShotgunPellet";
        // go_Bullet = (GameObject)Resources.Load(shotgunPelletFile, typeof(GameObject));

        // i_ShotsInMagazine = i_ShotsInMagazine_Max;

        // Connect to pivot for reloading
        go_WeaponModel = transform.Find("mdl_Static").gameObject;
        // go_PivotBall = go_WeaponModel.transform.Find("PivotBall").gameObject;
    }

    public void FireStaticGun()
    {
        // Don't let the gun fire if it's not ready
        if (WeaponState != WeaponState.Ready) return;

        print("Bang");
    }

    public override void Reload()
    {
        // Done because parent is 'virtual public void Reload()'
        base.Reload();

        /*
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
        else if (f_ReloadTimer < GunUpTime)
        {
            if (v3_PivotBallRot.x > 0f)
            {
                v3_PivotBallRot.x -= Time.deltaTime * 30f * 2f;

                if (f_ReloadTimer == 0f) v3_PivotBallRot.x = 0f;
            }
        }

        // Apply rotation
        go_PivotBall.transform.localEulerAngles = v3_PivotBallRot;
        */
    }
}
