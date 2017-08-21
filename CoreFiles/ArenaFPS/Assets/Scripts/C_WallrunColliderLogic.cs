using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WallrunColliderLogic : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        i_LayerMask = LayerMask.NameToLayer("Wallrun");

        go_Collider = null;
    }

    GameObject go_Collider;
    public GameObject WallrunCollider
    {
        get { return go_Collider; }
    }

    Vector3 v3_ClosestContactPoint;
    public Vector3 ClosestContactPoint
    {
        get { return v3_ClosestContactPoint; }
    }


    int i_LayerMask;
    private void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject.layer == i_LayerMask)
        {
            go_Collider = collider_.gameObject;
        }
    }

    private void OnTriggerStay(Collider collider_)
    {
        if(collider_.gameObject == go_Collider)
        {
            v3_ClosestContactPoint = collider_.ClosestPointOnBounds(gameObject.transform.position);
        }
    }

    private void OnTriggerExit(Collider collider_)
    {
        if (collider_.gameObject == go_Collider)
        {
            go_Collider = null;
            v3_ClosestContactPoint = new Vector3();
        }
    }
}
