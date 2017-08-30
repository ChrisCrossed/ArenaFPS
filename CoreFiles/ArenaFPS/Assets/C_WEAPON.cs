using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WEAPON : MonoBehaviour
{
    WeaponState weaponState = WeaponState.Ready;

    // Use this for initialization
    void Start ()
    {
		
	}

    public WeaponState WeaponState
    {
        set
        {
            if (weaponState != value)
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

    internal float f_WeaponState_Timer;
    internal float f_WeaponState_Timer_Max = 1.0f;
    internal float f_WeaponDisableRotation = 60f;
    public void SetWeaponState(WeaponState weaponState_)
    {
        if (weaponState_ == WeaponState.Enable)
        {
            f_WeaponState_Timer = f_WeaponState_Timer_Max;
        }
        else if (weaponState_ == WeaponState.Disable)
        {
            f_WeaponState_Timer = 0f;
        }
    }

    internal GameObject go_PivotBall;
    internal float f_ReloadTimer;
    internal float f_FireDelay;
    // Number of shells before reloading
    internal int i_ShotsInMagazine;
    internal int i_ShotsInMagazine_Max;
    public void Reload()
    {
        // Reduce timer
        if (f_ReloadTimer > 0)
        {
            f_ReloadTimer -= Time.deltaTime;

            if (f_ReloadTimer < 0)
            {
                f_ReloadTimer = 0f;
                WeaponState = WeaponState.Ready;
                i_ShotsInMagazine = i_ShotsInMagazine_Max;
            }
        }
    }

    public void Update()
    {
        
    }
}
