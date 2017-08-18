using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BallMovement : MonoBehaviour
{
    float f_yPos;

	// Use this for initialization
	void Start ()
    {
        f_yPos = gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 v3_Pos = gameObject.transform.position;
        v3_Pos.y = Mathf.Sin(Time.time * 2.0f) * 0.25f;
        v3_Pos.y += f_yPos;
        gameObject.transform.position = v3_Pos;
	}
}
