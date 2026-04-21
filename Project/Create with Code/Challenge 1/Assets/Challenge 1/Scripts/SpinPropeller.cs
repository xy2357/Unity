using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinPropeller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the propeller at a constant speed
        transform.Rotate(Vector3.forward, 200f * Time.deltaTime);
    }
}
