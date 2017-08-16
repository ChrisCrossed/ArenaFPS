using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerController : C_INPUT_MANAGER
{
    // Connections
    GameObject this_GameObject;
    Rigidbody this_RigidBody;
    GameObject this_CameraObject;

    // Character Movement
    [SerializeField] float f_MaxSpeed = 5.0f;
    [SerializeField] float f_CameraRot = 3.0f;
    [SerializeField] bool b_InvertedCamera = false;
    [SerializeField] float f_AirControlPercentage = 0.25f;

    // Various information
    float f_RaycastPoint_yPos;

	// Use this for initialization
	new void Start ()
    {
        // Connections
        this_GameObject = gameObject;
        this_RigidBody = this_GameObject.GetComponent<Rigidbody>();
        this_CameraObject = this_GameObject.transform.Find("Camera").gameObject;

        // Raycast Connections
        RaycastPoints[0] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_0").gameObject;
        RaycastPoints[1] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_1").gameObject;
        RaycastPoints[2] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_2").gameObject;
        RaycastPoints[3] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_3").gameObject;
        RaycastPoints[4] = this_GameObject.transform.Find("RaycastPoints").transform.Find("RaycastPoint_4").gameObject;

        base.Start();
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
            // Convert to ground normal
            v3_PlayerVelocity_ = Vector3.ProjectOnPlane(v3_PlayerVelocity_, -hit_.normal);

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
            f_yVel_ -= 50f * Time.deltaTime;
        }

        // If the player is moving faster than the maximum speed possible, cap it
        float f_CurrSpeed_ = v3_PlayerVelocity_.magnitude;
        if(f_CurrSpeed_ > f_MaxSpeed)
        {
            v3_PlayerVelocity_.Normalize();
            v3_PlayerVelocity_ *= f_MaxSpeed;
        }

        // Reapply y Velocity (gravity)
        v3_PlayerVelocity_.y = f_yVel_;

        // Lerp old player speed into new player speed
        v3_PlayerVelocity_ = Vector3.Lerp(v3_PlayerVelocity_Old, v3_PlayerVelocity_, 0.25f);

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

    GameObject[] RaycastPoints = new GameObject[5];
    bool RaycastToGround(out RaycastHit hit_)
    {
        // Reset values
        hit_ = new RaycastHit();
        bool b_HitGround_ = false;
        int i_LayerMask_ = LayerMask.GetMask("Ground");
        float f_DistToGround_ = 0.1f;

        // If the player is close enough to touching the ground, report it
        if(Physics.Raycast(RaycastPoints[0].transform.position, Vector3.down, out hit_, f_DistToGround_, i_LayerMask_))
        {
            b_HitGround_ = true;
        }

        // Apply natural gravity (although we're messing with it) if touching the ground
        this_RigidBody.useGravity = !b_HitGround_;

        // Return values
        return b_HitGround_;
    }

    float f_JumpTimer = 0.1f;
    static float f_JumpTimer_Max = 5.0f; // Consider making this the time for the JumpJet to 'refuel'
    private void FixedUpdate()
    {
        // Reduce jump timer
        if (f_JumpTimer > 0f)
        {
            f_JumpTimer -= Time.fixedDeltaTime;
            if (f_JumpTimer <= 0f) f_JumpTimer = 0f;
        }
        
        PlayerInput();

        CameraInput();
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();
    }
}
