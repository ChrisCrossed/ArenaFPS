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
    TeamColor enemyTeamColor;

    // Balance Data (DO NOT SET HERE)
    int DamagePerPellet;

    public void SpawnBullet(float f_Speed_, GameObject go_Owner_, GameObject go_ProjectilePoint_, TeamColor teamColor_, int DamagePerPellet_, float PelletSpread_ )
    {
        // Set Enemy Team Color
        enemyTeamColor = TeamColor.Red;
        if (teamColor_ == TeamColor.Red) enemyTeamColor = TeamColor.Blue;

        // Set Speed
        f_Speed = f_Speed_;

        // Set Owner
        go_Owner = go_Owner_;

        DamagePerPellet = DamagePerPellet_;

        SetDirection(go_ProjectilePoint_.transform.forward, PelletSpread_);
    }

    public void SetDirection(Vector3 _forward, float _intensity)
    {
        Vector3 _direction = _forward + Random.onUnitSphere * (_intensity / 90);

        if (_intensity == 0f) _direction = _forward;

        transform.LookAt(transform.position + _direction);

        // base.RaycastBullet(BulletCreationPoint(), _direction);
    }

    private void OnTriggerEnter(Collider collider_)
    {
        if (collider_.gameObject.GetComponent<C_PlayerController>())
        {
            GameObject go_Player = collider_.gameObject;
            C_PlayerController playerController = go_Player.GetComponent<C_PlayerController>();

            // If hits a player
            if (playerController.TeamColor == enemyTeamColor)
            {
                playerController.ApplyDamage(go_Owner, DamagePerPellet);
            }    
        }

        if (collider_.gameObject.layer == LayerMask.NameToLayer("Bullet")) return;

        // If it hits any (other) obstacle
        DestroyBullet();
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
