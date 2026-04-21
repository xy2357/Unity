using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public GameObject dogPrefab;
    private float lastSpawnTime = 0f;
    private float spawnCooldown = 2.0f;

    // Update is called once per frame
    void Update()
    {
        // On spacebar press, send dog (with cooldown)
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastSpawnTime + spawnCooldown)
        {
            Instantiate(dogPrefab, transform.position, dogPrefab.transform.rotation);
            lastSpawnTime = Time.time;
        }
    }
}
