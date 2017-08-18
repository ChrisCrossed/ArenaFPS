using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerController : C_INPUT_MANAGER
{
    // Connections
    GameObject this_GameObject;
    Rigidbody this_RigidBody;
    GameObject this_CameraObject;
    GameObject[] this_WallrunCollider = new GameObject[2];

    // Character Movement
    [SerializeField] float f_MaxSpeed = 5.0f;
    [SerializeField] float f_CameraRot = 3.0f;
    [SerializeField] bool b_InvertedCamera = false;
    [SerializeField] float f_AirControlPercentage = 0.25f;
    
    // Various information
    float f_RaycastPoint_yPos;

    // Objective Control
    bool b_HasObjective;

	// Use this for initialization
	new void Start ()
    {
        // Connections
        this_GameObject = gameObject;
        this_RigidBody = this_GameObject.GetComponent<Rigidbody>();
        this_CameraObject = this_GameObject.transform.Find("Camera").gameObject;
        this_WallrunCollider[0] = this_GameObject.transform.Find("WallrunCollider_Top").gameObject;
        this_WallrunCollider[1] = this_GameObject.transform.Find("WallrunCollider_Bottom").gameObject;
        colliderBot = this_WallrunCollider[0].GetComponent<C_WallrunColliderLogic>();
        colliderTop = this_WallrunCollider[1].GetComponent<C_WallrunColliderLogic>();

        // Raycast Connections
        RaycastPoints[0] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_0").gameObject;
        RaycastPoints[1] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_1").gameObject;
        RaycastPoints[2] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_2").gameObject;
        RaycastPoints[3] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_3").gameObject;
        RaycastPoints[4] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_4").gameObject;

        // Objective Models
        BallModel = transform.Find("BallModel");

        HasObjective = false;

        base.Start();
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

        Vector3 v3_Velocity_ = this_RigidBody.velocity;
        v3_Velocity_.y = 10f;

        f_JumpTimer = f_JumpTimer_Max;

        return v3_Velocity_;
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
        if (playerInput.Button_A == XInputDotNetPure.ButtonState.Pressed && f_JumpTimer == 0f && b_TouchingGround)
        {
            v3_PlayerVelocity_ = JumpVelocity();
        }

        // Input velocity to player object
        this_RigidBody.velocity = v3_PlayerVelocity_;

        // Store old velocity for reference
        v3_PlayerVelocity_Old = v3_PlayerVelocity_;
    }

    float f_VertAngle;
    float f_VertAngle_Old;
    void CameraInput()
    {
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
        f_VertAngle_Old = f_VertAngle;

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

    float f_JumpTimer = 0.1f;
    static float f_JumpTimer_Max = 1.0f; // (5.0) Consider making this the time for the JumpJet to 'refuel'
    private void FixedUpdate()
    {
        // Reduce jump timer
        if (f_JumpTimer > 0f)
        {
            f_JumpTimer -= Time.fixedDeltaTime;
            if (f_JumpTimer <= 0f) f_JumpTimer = 0f;
        }
        
        PlayerInput();

        if(playerInput.Trigger_Left == XInputDotNetPure.ButtonState.Pressed && !b_TouchingGround)
        {
            CaptureWallrunColliders();
        }

        CameraInput();
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape) || playerInput.Button_Start == XInputDotNetPure.ButtonState.Pressed)
            Application.Quit();
    }
}
