using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnPoint : MonoBehaviour
{
    // Team Side
    [SerializeField] TeamColor teamSide = TeamColor.Red;

    // Connection to System Manager
    GameObject go_SystemManager;
    C_SystemManager SystemManager;

	// Use this for initialization
	void Start ()
    {
        go_SystemManager = GameObject.Find("SystemManager");
        SystemManager = go_SystemManager.GetComponent<C_SystemManager>();

        PlayerPositions = new List<Vector3>();

        RelayExistenceToSystemManager();

        VisualState = false;
    }

    bool b_VisualState;
    public bool VisualState
    {
        set
        {
            b_VisualState = value;

            // Set sphere child to state
            transform.GetChild(0).GetComponent<SphereCollider>().enabled = b_VisualState;
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = b_VisualState;

            // Set mesh renderer to state
            gameObject.GetComponent<MeshRenderer>().enabled = b_VisualState;
        }

        get { return b_VisualState; }
    }

    List<Vector3> PlayerPositions;
    static float MaxPlayerDistance = 15f;
    float EnemyScoreMultiplier = 100f;
    float TeammateScoreMultiplier = 25f;
    public float GetSpawnScore
    {
        get
        {
            // Start value at 0f;
            float f_SpawnScore_ = 0f;

            // Find all enemies within a set radius
            if (teamSide == TeamColor.Red)
                PlayerPositions = SystemManager.GetTeamPositions(TeamColor.Blue);
            else
                PlayerPositions = SystemManager.GetTeamPositions(TeamColor.Red);

            // Decrease score based on enemy distance
            foreach (Vector3 EnemyPosition in PlayerPositions)
            {
                float f_Distance_ = Vector3.Distance(EnemyPosition, gameObject.transform.position);
                if (f_Distance_ < MaxPlayerDistance)
                {
                    // Don't allow players to spawn on top of each other
                    if (f_Distance_ < 2.0f)
                    {
                        f_SpawnScore_ = -1000f;
                    }
                    else
                    {
                        float f_InvertPerc = 1 - (f_Distance_ / MaxPlayerDistance);

                        // Reduce score based on proximity of enemies
                        f_SpawnScore_ -= f_InvertPerc * EnemyScoreMultiplier;
                    }
                }
            }

            // Find all Teammates within a set radius
            PlayerPositions = SystemManager.GetTeamPositions(teamSide);

            // Increase score based on teammate distance
            foreach (Vector3 BuddyPosition in PlayerPositions)
            {
                float f_Distance_ = Vector3.Distance(BuddyPosition, gameObject.transform.position);
                if(f_Distance_ < MaxPlayerDistance)
                {
                    // Don't allow players to spawn on top of each other
                    if(f_Distance_ < 2.0f)
                    {
                        f_SpawnScore_ = -1000f;
                    }
                    // Teammates are a reasonable distance away
                    else
                    {
                        float f_InvertPerc = 1 - (f_Distance_ / MaxPlayerDistance);

                        // Increase score based on proximity of buddies
                        f_SpawnScore_ += f_InvertPerc * TeammateScoreMultiplier;
                    }
                }
            }

            return f_SpawnScore_;
        }
    }

    public Vector3 GetPosition
    {
        get { return gameObject.transform.position; }
    }

    void RelayExistenceToSystemManager()
    {
        // Tell the System Manager we exist and what team
        SystemManager.ReceiveSpawnPoint(gameObject, teamSide);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (teamSide == TeamColor.Blue) Gizmos.color = Color.blue;

        Vector3 groundPosition = transform.position;
        groundPosition.y -= 1f;

        Gizmos.DrawWireSphere(groundPosition, MaxPlayerDistance);
    }
}
