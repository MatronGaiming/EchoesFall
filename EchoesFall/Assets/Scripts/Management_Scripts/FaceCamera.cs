using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        Vector3 direction = cameraPos - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
