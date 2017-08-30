using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WEAPON : MonoBehaviour
{
    #region Patch Settings
    protected float f_ReloadTimer;
    protected float f_FireDelay;

    #endregion
    WeaponState weaponState = WeaponState.Ready;
    
    // Weapon Manager
    protected C_WEAPONMANAGER WeaponManager;

    // Store player who fired
    protected Transform t_Player;

    // Use this for initialization
    void Start ()
    {
        t_Player = transform.parent.transform.parent;
        WeaponManager = t_Player.GetComponent<C_WEAPONMANAGER>();
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

    protected float f_WeaponState_Timer;
    protected float f_WeaponState_Timer_Max = 1.0f;
    protected float f_WeaponDisableRotation = 60f;
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

    protected GameObject go_PivotBall;

    // Number of shells before reloading
    protected int i_ShotsInMagazine;
    protected int i_ShotsInMagazine_Max;
    virtual public void Reload() // Child uses 'public override void Reload()' with 'base.Reload()'`
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
    
    // Weapon Model
    protected GameObject go_WeaponModel;
    void UpdateWeaponPosition(float f_Perc_)
    {
        Vector3 v3_Rot = go_WeaponModel.transform.localEulerAngles;
        v3_Rot.x = f_Perc_ * f_WeaponDisableRotation;
        go_WeaponModel.transform.localEulerAngles = v3_Rot;
    }

    public void Update()
    {
        if (f_FireDelay > 0f)
        {
            f_FireDelay -= Time.deltaTime;
            if (f_FireDelay < 0f) f_FireDelay = 0f;
        }

        if (WeaponState == WeaponState.Reloading)
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

                // Rotate game object based on percentage
                float f_Perc = f_WeaponState_Timer / f_WeaponState_Timer_Max;

                UpdateWeaponPosition(f_Perc);
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
                UpdateWeaponPosition(f_Perc);
            }
        }
    }
}
