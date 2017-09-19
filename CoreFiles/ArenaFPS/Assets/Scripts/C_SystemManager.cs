using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SystemManager : MonoBehaviour
{
    // Connect to Ball Object (If it exists)
    GameObject go_Ball;
    C_BallLogic ballLogic;

    // Team Scores
    int i_Score_Blue;
    int i_Score_Red;
    // [SerializeField] int i_Score_Max = 3;

    private void Awake()
    {
        if (GameObject.Find("BallSpawn"))
        {
            go_Ball = GameObject.Find("BallSpawn");
            ballLogic = go_Ball.GetComponent<C_BallLogic>();
        }

        GetAllPlayerObjects();
        SpawnPositions_Red = new List<GameObject>();
        SpawnPositions_Blue = new List<GameObject>();
    }

    // Use this for initialization
    private IEnumerator coroutine;
    void Start ()
    {
        coroutine = StartGame();
        StartCoroutine(coroutine);
    }

    public void SetBallState(bool b_IsActive_)
    {
        if(ballLogic)
        {
            ballLogic.IsActive = b_IsActive_;
        }
    }

    public void Score(int i_Points_, TeamColor teamColor_)
    {
        SetBallState(true);

        #region Score Text on Console
        string ScoreOutput;

        // Print scoring team name
        if (teamColor_ == TeamColor.Red)
        {
            ScoreOutput = "Blue Team ";
        }
        else
        {
            ScoreOutput = "Red Team ";
        }

        // Apply score
        print(ScoreOutput + "Scored " + i_Points_ + " points");
        #endregion
    }

    void GetAllPlayerObjects()
    {
        // Reset all Team Objects
        BlueTeam = new List<GameObject>();
        TeamPositions_Blue = new List<Vector3>();
        RedTeam = new List<GameObject>();
        TeamPositions_Red = new List<Vector3>();

        // Find all objects with a C_PlayerController script
        C_PlayerController[] go_PlayerList = FindObjectsOfType(typeof(C_PlayerController)) as C_PlayerController[];

        // Separate by team
        foreach (C_PlayerController playerController in go_PlayerList)
        {
            if (playerController.TeamColor == TeamColor.Red)
            {
                // Add the player itself to the Team List
                RedTeam.Add(playerController.transform.gameObject);

                // Store the position of the player
                TeamPositions_Red.Add(playerController.transform.position);
            }
            else
            {
                // Add the player itself to the Team List
                BlueTeam.Add(playerController.transform.gameObject);

                // Store the position of the player
                TeamPositions_Blue.Add(playerController.transform.position);
            }
        }
    }

    // Gather all unit's physical position
    List<GameObject> BlueTeam;
    List<GameObject> RedTeam;
    List<Vector3> TeamPositions_Blue;
    List<Vector3> TeamPositions_Red;
    float f_ScrapeTimer;
    static float f_ScrapeTimer_Max = 0.05f;
    void ScrapeUnitPositions()
    {
        f_ScrapeTimer += Time.deltaTime;

        if(f_ScrapeTimer > f_ScrapeTimer_Max)
        {
            // Reset Scrape Timer
            f_ScrapeTimer = 0f;

            // Gather all Blue Player positions
            for(int i = 0; i < BlueTeam.Count; ++i)
            {
                TeamPositions_Blue[i] = BlueTeam[i].transform.position;
            }

            // Gather all Red Player Positions
            for(int j = 0; j < RedTeam.Count; ++j)
            {
                TeamPositions_Red[j] = BlueTeam[j].transform.position;
            }
        }
    }

    public List<Vector3> GetTeamPositions(TeamColor teamColor_)
    {
        if(teamColor_ == TeamColor.Red)
        {
            // Red Team
            return TeamPositions_Red;
        }

        // Blue Team
        return TeamPositions_Blue;
    }
    
    List<GameObject> SpawnPositions_Red;
    List<GameObject> SpawnPositions_Blue;
    public void ReceiveSpawnPoint(GameObject spawnPosition_, TeamColor teamColor_)
    {
        if (teamColor_ == TeamColor.Blue)
            SpawnPositions_Blue.Add(spawnPosition_);
        else
            SpawnPositions_Red.Add(spawnPosition_);
    }

    public GameObject PlayerRequestsSpawn( TeamColor teamColor_ )
    {
        // Storage for the final spawn position
        GameObject go_SpawnPosition_ = null;

        // Set the current evaluated score to be super low
        float f_CurrScore_ = -1000f;

        // Temporary container for the positions to be evaluated
        List<GameObject> teamPositions = new List<GameObject>();
        
        // Set current list to run through
        if (teamColor_ == TeamColor.Red)
            teamPositions = SpawnPositions_Red;
        else
            teamPositions = SpawnPositions_Blue;

        // Run through all spawn points for that team
        foreach(GameObject spawnPoint in teamPositions)
        {
            // get the current score for this spawn point
            float f_TempScore_ = spawnPoint.GetComponent<C_SpawnPoint>().GetSpawnScore;

            // If the value is greater, swap
            if(f_TempScore_ > f_CurrScore_)
            {
                // Catch new score
                f_CurrScore_ = f_TempScore_;

                // Assign new final spawn position
                go_SpawnPosition_ = spawnPoint;
            }
        }

        // Return the position we found
        return go_SpawnPosition_;
    }

    private void LateUpdate()
    {
        ScrapeUnitPositions();
    }

    private IEnumerator StartGame()
    {
        while(true)
        {
            // WHY DOES THIS WORK
            yield return new WaitForSeconds(0);

            // Spawn all Blue Players
            for (int i_ = 0; i_ < BlueTeam.Count; ++i_)
                BlueTeam[i_].GetComponent<C_PlayerController>().SpawnPlayer();

            // Spawn all Red Players
            for (int j_ = 0; j_ < RedTeam.Count; ++j_)
                RedTeam[j_].GetComponent<C_PlayerController>().SpawnPlayer();

            // Break out
            break;
        }
    }
}