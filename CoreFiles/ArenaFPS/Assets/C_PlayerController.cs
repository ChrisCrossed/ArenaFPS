using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerController : C_INPUT_MANAGER
{
    // Connections
    GameObject this_GameObject;
    Rigidbody this_RigidBody;
    GameObject this_CameraObject;
    Camera this_Camera;
    GameObject this_RaycastPoint;

    // Character Movement
    [SerializeField] float f_MaxSpeed = 5.0f;
    [SerializeField] float f_CameraRot = 3.0f;
    [SerializeField] bool b_InvertedCamera = false;

	// Use this for initialization
	new void Start ()
    {
        // Connections
        this_GameObject = gameObject;
        this_RigidBody = this_GameObject.GetComponent<Rigidbody>();
        this_CameraObject = this_GameObject.transform.Find("Camera").gameObject;
        this_Camera = this_CameraObject.GetComponent<Camera>();
        this_RaycastPoint = this_GameObject.transform.Find("RaycastPoint").gameObject;

        base.Start();
	}

    float f_VertAngle;
    void PlayerInput()
    {
        // Capture player gravity velocity
        float f_yVel_ = this_RigidBody.velocity.y;

        // Rotate player based on Right Stick
        Vector3 v3_PlayerRot = this_GameObject.transform.eulerAngles;
        Vector3 v3_NewRot = v3_PlayerRot;
        v3_NewRot.y += playerInput.LookHoriz * f_CameraRot;
        v3_NewRot = Vector3.Lerp(v3_PlayerRot, v3_NewRot, 1.0f / Time.deltaTime);
        this_GameObject.transform.eulerAngles = v3_NewRot;

        // Rotate camera based on Right Stick
        f_VertAngle -= playerInput.LookVert;
        f_VertAngle = Mathf.Clamp(f_VertAngle, -89f, 89f);

        Vector3 v3_CamEuler = this_CameraObject.transform.eulerAngles;
        v3_CamEuler.x = f_VertAngle;
        this_CameraObject.transform.eulerAngles = v3_CamEuler;

        // Convert left Stick into velocity
        Vector3 v3_NewVel_ = this_GameObject.transform.rotation * new Vector3(playerInput.xDir, 0, playerInput.zDir) * f_MaxSpeed;
        v3_NewVel_.y = f_yVel_;
        this_RigidBody.velocity = v3_NewVel_;
        
    }

    bool b_IsTouchingGround;
    void RaycastToGround()
    {
        RaycastHit hit_;
        int i_LayerMask_ = LayerMask.GetMask("Ground");
        float f_DistToGround_ = 0.05f;
        b_IsTouchingGround = false;

        if(Physics.Raycast(this_RaycastPoint.transform.position, Vector3.down, out hit_, f_DistToGround_, i_LayerMask_))
        {
            b_IsTouchingGround = true;
        }

        print("Touching Ground: " + b_IsTouchingGround);

        this_RigidBody.useGravity = !b_IsTouchingGround;
    }

    private void FixedUpdate()
    {
        PlayerInput();
        RaycastToGround();
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();
    }
}
