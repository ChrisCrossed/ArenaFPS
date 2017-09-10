using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WeaponModelManager : C_INPUT_MANAGER
{
    private void Awake()
    {
        // Move all relevant objects first
        MoveShotgunRaycasters();
    }

    // Move Shotgun Raycast Data
    void MoveShotgunRaycasters()
    {
        Transform ShotgunProjectilePoint = transform.Find("mdl_Shotgun").transform.Find("ProjectilePoint");
        Transform ShotgunWeaponRaycastPoint = transform.Find("mdl_Shotgun").transform.Find("WeaponRaycastPoint");

        Vector3 ShotgunProjectilePointPosition = ShotgunProjectilePoint.transform.position;
        Vector3 ShotgunProjectilePointEulers = ShotgunProjectilePoint.transform.eulerAngles;

        Vector3 ShotgunWeaponRaycastPointPosition = ShotgunWeaponRaycastPoint.transform.position;
        Vector3 ShotgunWeaponRaycastPointEulers = ShotgunWeaponRaycastPoint.transform.eulerAngles;

        // Find parent object (Moving up a layer)
        Transform RaycastObjectsParent = transform.parent;
        print("Moved to: " + RaycastObjectsParent.name);

        // Move down one layer to RaycastObjects
        Transform RaycastObjects = RaycastObjectsParent.Find("RaycastObjects");
        print("Moved to: " + RaycastObjects.name);

        // Move to proper position
        ShotgunProjectilePoint.SetParent(RaycastObjects);
        ShotgunWeaponRaycastPoint.SetParent(RaycastObjects);

        // Reset position & rotation
        ShotgunProjectilePoint.transform.position = ShotgunProjectilePointPosition;
        ShotgunProjectilePoint.transform.eulerAngles = ShotgunProjectilePointEulers;

        ShotgunWeaponRaycastPoint.transform.position = ShotgunWeaponRaycastPointPosition;
        ShotgunWeaponRaycastPoint.transform.eulerAngles = ShotgunWeaponRaycastPointEulers;
    }

    void MoveWeaponModels()
    {
        // Get local position
        Vector3 localPosition = gameObject.transform.localPosition;
        Quaternion localRotation = gameObject.transform.localRotation;

        // Change transform to new Camera model
        GameObject weaponHUD;
        if (player == XInputDotNetPure.PlayerIndex.One)
            weaponHUD = GameObject.Find("WeaponHUD_PlayerOne");
        else if(player == XInputDotNetPure.PlayerIndex.Two)
            weaponHUD = GameObject.Find("WeaponHUD_PlayerTwo");
        else if(player == XInputDotNetPure.PlayerIndex.Three)
            weaponHUD = GameObject.Find("WeaponHUD_PlayerThree");
        else
            weaponHUD = GameObject.Find("WeaponHUD_PlayerFour");

        gameObject.transform.SetParent(weaponHUD.transform);

        // Set local position
        gameObject.transform.localPosition = localPosition;
        gameObject.transform.localRotation = localRotation;
    }

    float WaitTimer;
    private void Update()
    {
        if(WaitTimer < 0.05f)
        {
            WaitTimer += Time.deltaTime;

            if(WaitTimer > 0.05f)
            {
                // Move remaining weapons
                MoveWeaponModels();
            }
        }
    }
}
