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

        base.Start();
	}
    float f_VertAngle;
    void PlayerInput()
    {
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
        this_RigidBody.velocity = this_GameObject.transform.rotation * new Vector3(playerInput.xDir, 0, playerInput.zDir) * f_MaxSpeed;
    }

    private void FixedUpdate()
    {
        PlayerInput();
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();
    }
}
