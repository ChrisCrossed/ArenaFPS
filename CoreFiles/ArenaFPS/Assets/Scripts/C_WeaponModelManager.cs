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

        // Move down one layer to RaycastObjects
        Transform RaycastObjects = RaycastObjectsParent.Find("RaycastObjects");

        // Move to proper position
        ShotgunProjectilePoint.SetParent(RaycastObjects);
        ShotgunWeaponRaycastPoint.SetParent(RaycastObjects);

        // Reset position & rotation
        ShotgunProjectilePoint.transform.position = ShotgunProjectilePointPosition;
        ShotgunProjectilePoint.transform.eulerAngles = ShotgunProjectilePointEulers;

        ShotgunWeaponRaycastPoint.transform.position = ShotgunWeaponRaycastPointPosition;
        ShotgunWeaponRaycastPoint.transform.eulerAngles = ShotgunWeaponRaycastPointEulers;
    }
}
