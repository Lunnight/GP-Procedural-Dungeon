using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y + 1f, transform.parent.position.z);
        transform.rotation = transform.parent.rotation;
    }
}
