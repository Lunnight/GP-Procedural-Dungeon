using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStairs : MonoBehaviour
{
    public GameObject stairs;
    FloorOverhead overheadSpawner;
    void Start()
    {
        overheadSpawner = GameObject.FindObjectOfType<FloorOverhead>();
        StartCoroutine(Waiting());
    }
    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator Waiting()
    {
        Vector3 roomPosition;

        yield return new WaitForSeconds(5);
        roomPosition = overheadSpawner.RoomPositions[Random.Range(0, overheadSpawner.RoomPositions.Count)];

        Instantiate(stairs, new Vector3(roomPosition.x, 0.25f, roomPosition.z), new Quaternion(0, 0, 0, 0));
        Destroy(gameObject);
    }
}
