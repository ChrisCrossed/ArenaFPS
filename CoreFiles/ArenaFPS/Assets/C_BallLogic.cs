using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BallLogic : MonoBehaviour
{
    int i_LayerMask;

    // Model
    GameObject go_Model;
    MeshRenderer this_MeshRenderer;

    // Collider
    BoxCollider this_BoxCollider;

	// Use this for initialization
	void Start ()
    {
        // Player layer mask
        i_LayerMask = LayerMask.NameToLayer("Player");

        go_Model = transform.Find("Model").gameObject;
        this_MeshRenderer = go_Model.GetComponent<MeshRenderer>();

        this_BoxCollider = gameObject.GetComponent<BoxCollider>();
	}

    private void OnTriggerEnter(Collider collider_)
    {
        if(collider_.gameObject.layer == i_LayerMask)
        {
            // Tell the player that touched it they 'own the ball'
            collider_.gameObject.GetComponent<C_PlayerController>().HasObjective = true;

            // Set ball state
            IsActive = false;
        }
    }

    bool b_IsActive;
    bool IsActive
    {
        set
        {
            b_IsActive = value;

            SetActive(b_IsActive);
        }
        get { return b_IsActive; }
    }

    void SetActive(bool b_IsActive_)
    {
        // Disable collider
        this_BoxCollider.enabled = b_IsActive_;

        // Turn off model mesh renderer
        this_MeshRenderer.enabled = b_IsActive_;
    }
}