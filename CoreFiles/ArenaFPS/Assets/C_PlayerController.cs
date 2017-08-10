using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerController : C_INPUT_MANAGER
{
    
	// Use this for initialization
	new void Start ()
    {
        base.Start();

        // SetPlayerIndex = XInputDotNetPure.PlayerIndex.One;
	}
	
	// Update is called once per frame
	new void Update ()
    {
        base.Update();

        print(playerInput.Button_A);
    }
}
