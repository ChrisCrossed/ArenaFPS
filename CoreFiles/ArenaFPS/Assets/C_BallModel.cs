using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BallModel : MonoBehaviour
{
    float f_yPos;

    bool IsActive
    {
        set;
        get;
    }

    public void SetActive(bool IsActive_)
    {
        IsActive = IsActive_;

        gameObject.GetComponent<MeshRenderer>().enabled = IsActive;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float f_yPos = transform.parent.gameObject.transform.position.y;

        Vector3 v3_Pos = gameObject.transform.position;
        v3_Pos.y = (Mathf.Sin(Time.time * 2.0f) * 0.125f) + f_yPos + 1.5f;
        gameObject.transform.position = v3_Pos;
    }
}
