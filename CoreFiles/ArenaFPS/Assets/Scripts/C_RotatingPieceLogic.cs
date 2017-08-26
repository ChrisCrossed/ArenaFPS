using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_RotatingPieceLogic : MonoBehaviour
{
    enum CurrentState
    {
        Zero,
        One,
        Two,
        Three
    }

    [SerializeField] float AngledRotation = 30f;
    // float CurrentEndRotation;
    Rigidbody this_Rigidbody;

    // Hardcoded values
    float Angle_0;
    float Angle_1;
    float Angle_2;
    float Angle_3;

    // Current state
    CurrentState currentState = CurrentState.Zero;

	// Use this for initialization
	void Start ()
    {
        this_Rigidbody = gameObject.GetComponent<Rigidbody>();

        // CurrentEndRotation = AngledRotation;

        // Hardcoded values
        Angle_0 = AngledRotation;
        Angle_1 = 180f;
        Angle_2 = 180f + AngledRotation;
        Angle_3 = 0;
    }

    // Update is called once per frame
    float f_TimeUntilNextMove = 2f;
    static float f_TimeUntilNextMove_Max = 2f;
    static float f_MoveSpeed = 15.0f;
	void Update ()
    {
        if(f_TimeUntilNextMove > 0)
        {
            f_TimeUntilNextMove -= Time.deltaTime;
            if (f_TimeUntilNextMove < 0) f_TimeUntilNextMove = 0f;
        }
        else
        {
            Vector3 v3_CurrentRotation = this_Rigidbody.transform.eulerAngles;

            v3_CurrentRotation.y += Time.deltaTime * f_MoveSpeed;

            switch (currentState)
            {
                case CurrentState.Zero:
                    if(v3_CurrentRotation.y > Angle_0)
                    {
                        v3_CurrentRotation.y = Angle_0;

                        f_TimeUntilNextMove = f_TimeUntilNextMove_Max;

                        currentState = CurrentState.One;
                    }
                    break;
                case CurrentState.One:
                    if (v3_CurrentRotation.y > Angle_1)
                    {
                        v3_CurrentRotation.y = Angle_1;

                        f_TimeUntilNextMove = f_TimeUntilNextMove_Max;

                        currentState = CurrentState.Two;
                    }
                    break;
                case CurrentState.Two:
                    if (v3_CurrentRotation.y > Angle_2)
                    {
                        v3_CurrentRotation.y = Angle_2;

                        f_TimeUntilNextMove = f_TimeUntilNextMove_Max;

                        currentState = CurrentState.Three;
                    }
                    break;
                case CurrentState.Three:
                    if (v3_CurrentRotation.y > 0 && v3_CurrentRotation.y < 0.5f)
                    {
                        v3_CurrentRotation.y = Angle_3;

                        f_TimeUntilNextMove = f_TimeUntilNextMove_Max;

                        currentState = CurrentState.One;
                    }
                    break;
                default:
                    break;
            }

            this_Rigidbody.transform.eulerAngles = v3_CurrentRotation;
        }
    }
}
