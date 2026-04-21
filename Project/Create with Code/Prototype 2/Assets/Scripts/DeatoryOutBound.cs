using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeatoryOutBound : MonoBehaviour
{
    public float upBound = 35f;
    public float downBound = -35f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > upBound)
        {
            Destroy(gameObject);
        }
        else if (transform.position.z < downBound)
        {
            Debug.Log("Game Over!");
            Destroy(gameObject);
        }   
    }
}
