using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region PATCH NOTES

/* ------------------------------
 *
 *  Version 0.0.3
 *  
 *  - Health and Armor
 *      - Health Max    = 100
 *      - Armor Max     = 50
 *      - For every point of damage received, Armor is reduced by 2 and Damage received is halved.
 *  
 *  - Weapons
 *      - Shotgun
 *          - 3 damage per pellet
 *          - 8 pellets per shot
 *          - 3.5 degrees of Pellet Spread
 *          - 3500 unit Pellet Speed
 *          - 4 shots per 'magazine'
 *          - 0.5 seconds between shots
 *          - 2 seconds reload time
 *          - Notes: If all pellets hit in one 'magazine', 96 damage is done.
*/

#endregion

public class C_PlayerController : C_INPUT_MANAGER
{
    // Connections
    GameObject this_GameObject;
    Rigidbody this_RigidBody;
    GameObject this_CameraObject;
    GameObject[] this_WallrunCollider = new GameObject[2];
    C_WEAPONMANAGER this_WeaponManager;
    GameObject go_WeaponObject;
    GameObject go_SystemManager;
    C_SystemManager this_SystemManager;
    C_HealthManager this_HealthManager;

    // Character Movement
    [SerializeField] float f_MaxSpeed = 10.0f;
    [SerializeField] float f_CameraRot = 3.0f;
    [SerializeField] bool b_InvertedCamera = false;
    [SerializeField] float f_AirControlPercentage = 0.25f;
    
    // Various information
    float f_RaycastPoint_yPos;

    // Objective Control
    bool b_HasObjective;

    [SerializeField] TeamColor teamColor = TeamColor.Red;

    private void Awake()
    {
        // Connections
        this_GameObject = gameObject;
        this_RigidBody = this_GameObject.GetComponent<Rigidbody>();
        this_CameraObject = this_GameObject.transform.Find("Camera").gameObject;
        this_WallrunCollider[0] = this_GameObject.transform.Find("WallrunCollider_Top").gameObject;
        this_WallrunCollider[1] = this_GameObject.transform.Find("WallrunCollider_Bottom").gameObject;
        colliderBot = this_WallrunCollider[0].GetComponent<C_WallrunColliderLogic>();
        colliderTop = this_WallrunCollider[1].GetComponent<C_WallrunColliderLogic>();
        go_WeaponObject = this_CameraObject.transform.Find("Weapons").gameObject;
        this_WeaponManager = this_GameObject.GetComponent<C_WEAPONMANAGER>();
        go_SystemManager = GameObject.Find("SystemManager");
        this_SystemManager = go_SystemManager.GetComponent<C_SystemManager>();
        this_HealthManager = this_GameObject.GetComponent<C_HealthManager>();
    }

    // Use this for initialization
    new void Start ()
    {
        // Raycast Connections
        RaycastPoints[0] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_0").gameObject;
        RaycastPoints[1] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_1").gameObject;
        RaycastPoints[2] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_2").gameObject;
        RaycastPoints[3] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_3").gameObject;
        RaycastPoints[4] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_4").gameObject;

        // Objective Models
        BallModel = transform.Find("BallModel");

        HasObjective = false;

        // Set Layer according to team
        TeamColor = teamColor;
        
        // Initialize playerInput_Old
        playerInput_Old = new PlayerInput();

        base.Start();
	}

    public TeamColor TeamColor
    {
        get { return teamColor; }
        set
        {
            teamColor = value;

            // Set layer
            gameObject.layer = LayerMask.NameToLayer("PlayerRed");
            if (teamColor == TeamColor.Blue) gameObject.layer = LayerMask.NameToLayer("PlayerBlue");

            // Set player material
            Color matColor = Color.red;
            if (teamColor == TeamColor.Blue) matColor = Color.cyan;
            gameObject.GetComponent<MeshRenderer>().material.color = matColor;
        }
    }

    Transform BallModel;
    public bool HasObjective
    {
        set
        {
            b_HasObjective = value;

            if(BallModel)
            {
                BallModel.GetComponent<C_BallModel>().SetActive( b_HasObjective );
            }
        }
        get { return b_HasObjective; }
    }

    Vector3 JumpVelocity()
    {
        // Set jump state
        b_TouchingGround = false;

        // Temporarily disable wallrun colliders
        SetWallrunColliderState = false;
        WallrunDisabledTimer = WallrunDisabledTimer_Max;
        // colliderTop.WallrunCollider = null;

        Vector3 v3_Velocity_ = this_RigidBody.velocity;
        v3_Velocity_.y = 10f;

        f_JumpTimer = f_JumpTimer_Max;

        return v3_Velocity_;
    }

    float WallrunDisabledTimer = 0f;
    static float WallrunDisabledTimer_Max = 0.1f;
    bool SetWallrunColliderState
    {
        set
        {
            colliderTop.enabled = value;
            colliderBot.enabled = value;
        }
        get
        {
            bool BothEnabled = false;

            if (colliderTop.enabled && colliderBot.enabled) BothEnabled = true;

            return BothEnabled;
        }
    }

    Vector3 v3_PlayerVelocity_Old;
    Vector3 v3_HorizRot_Old;
    bool b_TouchingGround;
    void PlayerInput()
    {
        // Capture player gravity velocity
        float f_yVel_ = this_RigidBody.velocity.y;

        // Convert left Stick into velocity
        Vector3 v3_PlayerVelocity_ = new Vector3();
        Vector3 v3_InputVelocity_ = this_GameObject.transform.rotation * new Vector3(playerInput.xDir, 0, playerInput.zDir) * f_MaxSpeed;

        if (b_TouchingGround) v3_PlayerVelocity_ = v3_InputVelocity_;
        else v3_PlayerVelocity_ = this_RigidBody.velocity + (v3_InputVelocity_ * f_AirControlPercentage);

        // Determine if touching ground
        RaycastHit hit_;
        b_TouchingGround = RaycastToGround(out hit_);
        if (b_TouchingGround)
        {
            // Move player to be on the ground
            Vector3 v3_NewPos_ = hit_.point;
            v3_NewPos_.y += 1f;
            this_RigidBody.transform.position = v3_NewPos_;

            // Reset y Velocity
            f_yVel_ = 0f;
            v3_PlayerVelocity_Old.y = 0f;
        }
        else // Not touching the ground
        {
            f_yVel_ -= 65f * Time.deltaTime;
        }

        // If the player is moving faster than the maximum speed possible, cap it
        float f_CurrSpeed_ = v3_PlayerVelocity_.magnitude;
        if(f_CurrSpeed_ > f_MaxSpeed)
        {
            v3_PlayerVelocity_.Normalize();
            v3_PlayerVelocity_ *= f_MaxSpeed;
        }

        // Reapply y Velocity (gravity)
        if(!b_TouchingGround) v3_PlayerVelocity_.y = f_yVel_;

        // Lerp old player speed into new player speed
        v3_PlayerVelocity_ = Vector3.Lerp(v3_PlayerVelocity_Old, v3_PlayerVelocity_, 0.25f);

        PhysicMaterial physMat_ = this_GameObject.GetComponent<CapsuleCollider>().material;
        physMat_.dynamicFriction = 0f;
        physMat_.frictionCombine = PhysicMaterialCombine.Minimum;
        physMat_.staticFriction = 0f;
        
        if (b_TouchingGround)
        {
            if(playerInput.xDir == 0f && playerInput.zDir == 0f)
            {
                physMat_.dynamicFriction = 1f;
                physMat_.frictionCombine = PhysicMaterialCombine.Maximum;
                physMat_.staticFriction = 1f;
                this_GameObject.GetComponent<CapsuleCollider>().material = physMat_;
            }
        }

        this_GameObject.GetComponent<CapsuleCollider>().material = physMat_;

        // Convert to ground normal
        v3_PlayerVelocity_ = Vector3.ProjectOnPlane(v3_PlayerVelocity_, -hit_.normal);

        GameObject go_DebugPoint = transform.Find("DebugPoint").gameObject;
        // Debug.DrawRay(go_DebugPoint.transform.position, -hit_.normal, Color.red);
        
        // If the player has pressed A and we're allowed to jump, jump
        if (playerInput.Button_A == XInputDotNetPure.ButtonState.Pressed && f_JumpTimer == 0f)
        {
            v3_PlayerVelocity_ = JumpVelocity();
        }

        // Input velocity to player object
        this_RigidBody.velocity = v3_PlayerVelocity_;

        // Store old velocity for reference
        v3_PlayerVelocity_Old = v3_PlayerVelocity_;
    }

    float f_VertAngle;
    void CameraInput()
    {
        // If the player is holding down the bumper to select a weapon, don't look around
        if (BumperHeld) return;

        // Rotate player based on Right Stick
        Vector3 v3_PlayerRot = this_GameObject.transform.eulerAngles;
        Vector3 v3_NewRot = v3_PlayerRot;
        v3_NewRot.y += playerInput.LookHoriz * f_CameraRot;
        v3_NewRot = Vector3.Lerp(v3_PlayerRot, v3_NewRot, 1.0f / Time.deltaTime);
        this_GameObject.transform.eulerAngles = v3_NewRot;

        // Rotate camera based on Right Stick
        if (b_InvertedCamera) playerInput.LookVert *= -1f;
        f_VertAngle -= playerInput.LookVert * f_CameraRot;
        f_VertAngle = Mathf.Clamp(f_VertAngle, -89f, 89f);

        Vector3 v3_CamEuler = this_CameraObject.transform.eulerAngles;
        v3_CamEuler.x = f_VertAngle;
        this_CameraObject.transform.eulerAngles = v3_CamEuler;
    }

    C_WallrunColliderLogic colliderBot;
    C_WallrunColliderLogic colliderTop;
    void CaptureWallrunColliders()
    {
        // If top and bottom colliders are touching the same wallrun, override velocity
        GameObject go_Top = colliderTop.WallrunCollider;
        GameObject go_Bot = colliderBot.WallrunCollider;

        if (go_Top == null) return;
        if (go_Bot == null) return;
        
        if(go_Bot == go_Top)
        {
            Vector3 v3_Velocity = this_RigidBody.velocity;
            v3_Velocity.y = 0f;

            // Determine vector toward wall
            Vector3 v3_WallVector = new Vector3(colliderTop.ClosestContactPoint.x, this_GameObject.transform.position.y, colliderTop.ClosestContactPoint.z);
            v3_WallVector = v3_WallVector - this_GameObject.transform.position;
            v3_WallVector.Normalize();

            RaycastHit wallhit_;
            Physics.Raycast(this_GameObject.transform.position, v3_WallVector, out wallhit_, 0.5f, LayerMask.NameToLayer("Wallrun"));

            v3_WallVector = Vector3.ProjectOnPlane(v3_Velocity, -wallhit_.normal);
            v3_WallVector.Normalize();
            v3_WallVector *= f_MaxSpeed;

            this_RigidBody.velocity = v3_WallVector;
        }
    }

    GameObject[] RaycastPoints = new GameObject[5];
    bool RaycastToGround(out RaycastHit hit_)
    {
        // Reset values
        hit_ = new RaycastHit();
        bool b_HitGround_ = false;
        int i_LayerMask_ = LayerMask.GetMask("Ground");
        float f_DistToGround_ = 0.151f;

        // If the player is close enough to touching the ground, report it
        if(Physics.Raycast(RaycastPoints[0].transform.position, Vector3.down, out hit_, f_DistToGround_, i_LayerMask_))
        {
            b_HitGround_ = true;
        }

        // Apply natural gravity (although we're messing with it) if touching the ground
        // this_RigidBody.useGravity = !b_HitGround_;
        this_RigidBody.useGravity = false;

        // Return values
        return b_HitGround_;
    }

    int i_Health;
    [SerializeField] int i_Health_Max = 105;
    int i_Armor;
    [SerializeField] int i_Armor_Max = 75;
    public void ApplyDamage( GameObject go_DamagingPlayer_, int i_DamageAmount_ )
    {
        int i_DamageToApply = i_DamageAmount_;

        // If the player has any armor remaining...
        if (i_Armor > 0 )
        {
            // Determine if player has more Armor or Damage received
            int i_DamageToReduce = i_DamageToApply;
            if (i_Armor < i_DamageToReduce) i_DamageToReduce = i_Armor;

            // For every point of armor that exists, Reduce one point of damage received and armor remaining
            i_Armor -= i_DamageToReduce;
            if (i_Armor < 0) i_Armor = 0;

            // Send Armor information to HUD
            this_HealthManager.SetArmorBar(i_Armor, i_Armor_Max);

            // The damage received by the player is equal to HALF of what the armor received.
            i_DamageToApply -= i_DamageToReduce / 2;
        }

        // Apply damage and determine if dead
        i_Health -= i_DamageToApply;
        if(i_Health <= 0)
        {
            KillPlayer();
        }

        // Send Health information to HUD
        this_HealthManager.SetHealthBar(i_Health, i_Health_Max);
        print("Player: " + gameObject.name +
            ", Health: " + i_Health + ", Armor: " + i_Armor);
    }
    
    public void SpawnPlayer()
    {
        // Player is no longer dead
        b_IsDead = false;

        #region Weapons
        // Reset weapons in Weapon Manager
        this_WeaponManager.DisableAllPrimaryGuns();

        // Reset 'Weapons Picked Up' list.
        WeaponsPickedUp = new bool[8];

        // Turn off all weapon icons
        for (int i_ = 0; i_ < 8; ++i_)
            this_WeaponManager.WeaponHUDSetWeaponActive(i_, false);

        // Give the player a shotgun
        PickWeaponUp(WeaponList.Shotgun);

        // Set player weapons
        this_WeaponManager.ReadyForNextWeapon();
        this_WeaponManager.Weapon = WeaponList.Shotgun;  // Current
        #endregion

        #region Colliders and position
        // Turn off player collider
        gameObject.GetComponent<Collider>().enabled = true;

        // Turn off player mesh renderer
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        GameObject spawnPoint = this_SystemManager.PlayerRequestsSpawn(TeamColor);

        // Set Position and Rotation
        this_GameObject.transform.position = spawnPoint.transform.position;
        this_GameObject.transform.rotation = spawnPoint.transform.rotation;

        // Enable weapons (models)
        go_WeaponObject.SetActive(true);
        this_CameraObject.transform.Find("Visors").gameObject.SetActive(true);

        // Reset camera rotation
        f_VertAngle = 0f;
        #endregion

        #region Health and Armor
        // Reset health and armor
        i_Health = i_Health_Max;
        i_Armor = i_Armor_Max / 2;

        // Reset health (and armor, for now)
        this_HealthManager.SetHealthBar(1.0f);
        this_HealthManager.SetArmorBar(i_Armor, i_Armor_Max);
        #endregion
    }

    public void PickWeaponUp(WeaponList weapon_)
    {
        PickWeaponUp((int)weapon_);
    }
    public void PickWeaponUp(int weaponPickedUp_)
    {
        // If the player doesn't own this weapon, let them pick it up
        if(!WeaponsPickedUp[weaponPickedUp_])
        {
            // Enable weapon boolean
            WeaponsPickedUp[weaponPickedUp_] = true;

            // Enable weapon icon
            this_WeaponManager.WeaponHUDSetWeaponActive(weaponPickedUp_, true);
        }
    }

    public bool DoesPlayerOwnWeapon(WeaponList weapon_)
    {
        // if the player already has the weapon, return true
        if(WeaponsPickedUp[(int)weapon_])
            return true;

        // All other conditions, return false
        return false;
    }

    bool[] WeaponsPickedUp;
    void SwitchToSelectedWeapon()
    {
        // If weapon choice is invalid, return out
        if (WeaponChoice < 0 || WeaponChoice > 7) return;

        // If weapon is not owned, then just call the weapon already enabled (hacky method, returning doesn't work)
        if (!WeaponsPickedUp[WeaponChoice]) WeaponChoice = (int)this_WeaponManager.Weapon;

        // Switch to OWNED weapon
        this_WeaponManager.SetNextGun((WeaponList)WeaponChoice);

        // Reset selection
        WeaponChoice = -1;
    }

    float f_DeathTimer;
    float f_DeathTimer_CurrMax = 3.0f;
    bool b_IsDead;
    void KillPlayer()
    {
        f_DeathTimer = f_DeathTimer_CurrMax;

        // Turn off player collider
        gameObject.GetComponent<Collider>().enabled = false;

        // Turn off player mesh renderer
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        // Disable weapons (models)
        go_WeaponObject.SetActive(false);
        this_CameraObject.transform.Find("Visors").gameObject.SetActive(false);
    }

    float f_JumpTimer = 0.1f;
    static float f_JumpTimer_Max = 1.0f; // (5.0) Consider making this the time for the JumpJet to 'refuel'
    PlayerInput playerInput_Old;
    private void FixedUpdate()
    {
        // Reduce jump timer
        if (f_JumpTimer > 0f)
        {
            f_JumpTimer -= Time.fixedDeltaTime;
            if (f_JumpTimer <= 0f) f_JumpTimer = 0f;
        }

        if (WallrunDisabledTimer > 0)
        {
            WallrunDisabledTimer -= Time.fixedDeltaTime;
            if (WallrunDisabledTimer < 0)
            {
                WallrunDisabledTimer = 0f;

                SetWallrunColliderState = true;
            }
        }
        
        if(!b_IsDead)
        {
            PlayerInput();

            if(playerInput.Trigger_Left == XInputDotNetPure.ButtonState.Pressed && !b_TouchingGround)
            {
                CaptureWallrunColliders();
            }

            // User holds down Left Bumper or 'Y' button
            if (playerInput.Button_Y == XInputDotNetPure.ButtonState.Pressed)
            {
                // print("Weapon Wheel");
            }

            if (playerInput.Button_X == XInputDotNetPure.ButtonState.Pressed && playerInput_Old.Button_X == XInputDotNetPure.ButtonState.Released)
            {
                this_WeaponManager.ReloadGun();
            }
            
            playerInput_Old = playerInput;

            CameraInput();
        }
    }

    // Update is called once per frame
    float BumperHeldTimer;
    static float BumperHeldTimerThreshhold = 0.25f;
    bool BumperHeld;
    int WeaponChoice = -1;
    void Update()
    {
        base.UpdatePlayerInput();

        if (playerInput.Trigger_Right == XInputDotNetPure.ButtonState.Pressed)
        {
            this_WeaponManager.FireGun();
        }

        if (Input.GetKeyDown(KeyCode.P))
            PickWeaponUp(WeaponList.StaticGun);

        #region Set Weapon HUD State

        #region HUD Background
        // Reset WeaponHUDState, just to be safe
        this_WeaponManager.WeaponHUDState = false;

        // If left or right bumper is tapped (not held), quickswitch weapons
        if (playerInput.Bumper_Left == XInputDotNetPure.ButtonState.Pressed || playerInput.Bumper_Right == XInputDotNetPure.ButtonState.Pressed)
        {
            BumperHeldTimer += Time.deltaTime;

            if (BumperHeldTimer > BumperHeldTimerThreshhold)
            {
                // Show Weapon Menu
                this_WeaponManager.WeaponHUDState = true;
            }
        }
        else if (BumperHeldTimer > 0f)
        {
            // if button is released before the threshhold, perform a quickswitch
            if (BumperHeldTimer < BumperHeldTimerThreshhold)
            {
                // Weapon Quickswitch
                if (this_WeaponManager.PreviousWeapon != WeaponList.None)
                    this_WeaponManager.QuickswitchWeapons();
            }

            // Reset timer
            BumperHeldTimer = 0f;
        }
        #endregion

        #region Highlight selected weapon via input
        // If Bumper is held down, state that.
        if (playerInput.Bumper_Left == XInputDotNetPure.ButtonState.Pressed || playerInput.Bumper_Right == XInputDotNetPure.ButtonState.Pressed)
        {
            if(!BumperHeld)
            {
                // Set state
                BumperHeld = true;

                // Reset Weapon Chosen
                WeaponChoice = -1;
            }
        }
        // Otherwise, state that the Bumper is not held down, and show no weapon is selected
        else
        {
            // Only run commands if the Bumper used to be held
            if(!BumperHeld)
            {
                // Switch to weapon last selected
                SwitchToSelectedWeapon();
            }

            // Release bumper state
            BumperHeld = false;

            // Reset Weapon icon choice
            this_WeaponManager.WeaponHUDChoice(-1);
        }

        if (BumperHeld)
        {
            float AnalogStickThreshhold = 0.25f;

            // Left side of right analog stick
            if(playerInput.LookHoriz < -AnalogStickThreshhold)
            {
                if (playerInput.LookVert > AnalogStickThreshhold)
                    WeaponChoice = 7;
                else if (playerInput.LookVert < -AnalogStickThreshhold)
                    WeaponChoice = 5;
                else
                    WeaponChoice = 6;
            }
            // Right side of Right analog stick
            else if(playerInput.LookHoriz > AnalogStickThreshhold)
            {
                if (playerInput.LookVert > AnalogStickThreshhold)
                    WeaponChoice = 1;
                else if (playerInput.LookVert < -AnalogStickThreshhold)
                    WeaponChoice = 3;
                else
                    WeaponChoice = 2;
            }
            else
            {
                if (playerInput.LookVert > AnalogStickThreshhold)
                    WeaponChoice = 0;
                else if (playerInput.LookVert < -AnalogStickThreshhold)
                    WeaponChoice = 4;
                else // Unique state. Does not record weapon choice.
                    this_WeaponManager.WeaponHUDChoice(9);
            }

            // Sets weapon icon 
            this_WeaponManager.WeaponHUDChoice(WeaponChoice);
        }
        #endregion

        #endregion

        if(f_DeathTimer > 0f)
        {
            f_DeathTimer -= Time.deltaTime;
            if(f_DeathTimer < 0f)
            {
                f_DeathTimer = 0;
                SpawnPlayer();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || playerInput.Button_Start == XInputDotNetPure.ButtonState.Pressed)
            Application.Quit();
    }
}
