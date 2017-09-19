using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WeaponPickupCollider : MonoBehaviour
{
    C_WeaponPickupLogic WeaponPickup;

    private void Start()
    {
        WeaponPickup = transform.parent.GetComponent<C_WeaponPickupLogic>();
    }

    private void OnTriggerEnter(Collider collider_)
    {
        // If the collider is a player
        bool playerBlue = collider_.gameObject.layer == LayerMask.NameToLayer("PlayerBlue");
        bool playerRed = collider_.gameObject.layer == LayerMask.NameToLayer("PlayerRed");

        // If a player...
        if (playerBlue || playerRed)
        {
            // Pass the WeaponPickup the player game object
            WeaponPickup.GivePlayerPickup(collider_.gameObject);
        }
    }
}
