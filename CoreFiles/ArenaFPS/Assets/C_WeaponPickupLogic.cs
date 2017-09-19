using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WeaponPickupLogic : MonoBehaviour
{
    // Components
    [SerializeField] AnimationCurve animationCurve;
    GameObject InnerArch;
    GameObject WeaponModelParent;

    // Collider
    GameObject go_Collider;
    Collider Collider;

    // Variables
    float InnerArchStartRotation;

    // Weapon To Give
    [SerializeField] WeaponList WeaponToGive;

    // Disable Timer
    [SerializeField] float TimeToDisable = 30f;
    float DisableTimer;

    private void Awake()
    {
        // Connect to Inner Arch
        InnerArch = transform.Find("InnerArch").gameObject;
        InnerArchStartRotation = InnerArch.transform.localEulerAngles.y;

        // Connect to weapon model
        WeaponModelParent = transform.Find("WeaponModel").gameObject;

        go_Collider = transform.Find("Collider").gameObject;
    }

    // Use this for initialization
    void Start ()
    {
        Collider = go_Collider.GetComponent<MeshCollider>();
    }

    public void GivePlayerPickup(GameObject player_)
    {
        // Give player the weapon
        C_PlayerController player = player_.GetComponent<C_PlayerController>();

        // if the player already owns the weapon, return out
        if (player.DoesPlayerOwnWeapon(WeaponToGive)) return;
        
        // Give the player the weapon
        player.PickWeaponUp(WeaponToGive);

        // Disable collider for time
        DisableTimer = TimeToDisable;
        Collider.enabled = false;

        // Turn off weapon
        WeaponModelParent.SetActive(false);
    }

    // Update is called once per frame
    float InnerArchRotationTimer;
	void Update ()
    {
        // If timer is active, reduce
        if(DisableTimer > 0f)
        {
            // Reduce timer
            DisableTimer -= Time.deltaTime;

            if(DisableTimer <= 0f)
            {
                // Cap
                DisableTimer = 0f;

                // Enable collider
                Collider.enabled = true;

                // Turn on weapon model
                WeaponModelParent.SetActive(true);
            }
        }

        #region Set new rotation of inner arch
        // Increment and cap rotation timer
        InnerArchRotationTimer += Time.deltaTime / 5f;
        if (InnerArchRotationTimer >= 1.0f) InnerArchRotationTimer = 1.0f;

        // Record current rotation
        Vector3 InnerArchRotation = InnerArch.transform.localEulerAngles;

        // Determine percentage based on animation curve
        float perc = animationCurve.Evaluate(InnerArchRotationTimer);

        #region Set New Rotation based on percentage
        // Set new Y rotation, add to starting rotation
        InnerArchRotation.y = Mathf.LerpUnclamped(0f, 1f, perc);
        
        // Multiply by 90 degrees
        InnerArchRotation.y *= 90f;
        
        // Add starting rotation
        InnerArchRotation.y += InnerArchStartRotation;
        #endregion

        // Set new final rotation
        InnerArch.transform.localEulerAngles = InnerArchRotation;

        // Reset if animation is complete
        if (InnerArchRotationTimer == 1.0f) InnerArchRotationTimer = 0f;
        #endregion
    }
}
