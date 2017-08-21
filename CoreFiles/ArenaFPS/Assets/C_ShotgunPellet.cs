using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_ShotgunPellet : MonoBehaviour
{
    // Speed
    float f_Speed;

    // Owner
    GameObject go_Owner;

    // Self-Destruct Timer
    float f_Timer;
    float f_Timer_Max = 5f;

    // Set color based on team color
    Material MatColor;

	// Use this for initialization
	void Start ()
    {
        // gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 1000.0f * Time.deltaTime;

        // SpawnBullet(1000, gameObject);

        
    }

    public void SetDirection(Vector3 _forward, float _intensity = 5f)
    {
        Vector3 _direction = _forward + Random.onUnitSphere * (_intensity / 90);

        if (_intensity == 0f) _direction = _forward;

        transform.LookAt(transform.position + _direction);

        // base.RaycastBullet(BulletCreationPoint(), _direction);
    }

    public void SpawnBullet(float f_Speed_, GameObject go_Owner_, GameObject go_ProjectilePoint_, TeamColor teamColor_ )
    {
        // if (teamColor_ == TeamColor.Blue) MatColor = (Material)Resources.Load("Materials/Shotgun/mat_Blue");
        // else MatColor = (Material)Resources.Load("Materials/Shotgun/mat_Red");
        // gameObject.GetComponent<TrailRenderer>().materials[0] = MatColor;

        // Set Speed
        f_Speed = f_Speed_;

        // Set Owner
        go_Owner = go_Owner_;

        SetDirection(go_ProjectilePoint_.transform.forward, 5f);

        // Fire
        // gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * f_Speed * Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        // If hits a player

        // If hits an obstacle
    }

    // Update is called once per frame
    void Update ()
    {
        f_Timer += Time.deltaTime;

        if(f_Timer > f_Timer_Max)
        {
            DestroyBullet();
        }
    }

    private void FixedUpdate()
    {
        // Fire
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * f_Speed * Time.deltaTime;
    }

    void DestroyBullet()
    {
        GameObject.Destroy(this);
    }
}
