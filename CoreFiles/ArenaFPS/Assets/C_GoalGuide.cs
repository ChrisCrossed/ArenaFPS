using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GoalGuide : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        // Turn off mesh renderer
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
