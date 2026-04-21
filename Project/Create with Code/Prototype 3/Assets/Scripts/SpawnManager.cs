using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstaclePrefab;
    public Vector3 spawnPos = new Vector3(25, 0, 0);
    private float delayTime = 2.0f;
    private float repeatTime = 2.0f;
    public PlayController playerControllerScript;

    // Start is called before the first frame up
    void Start()
    {
        InvokeRepeating("SpawnObstacle", delayTime, repeatTime);
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnObstacle()
    {
        if (playerControllerScript.gameOver == false)
        {
            int obstacleIndex = Random.Range(0, obstaclePrefab.Length);
            Instantiate(obstaclePrefab[obstacleIndex], spawnPos, obstaclePrefab[obstacleIndex].transform.rotation);
        }
        
    }   
}
