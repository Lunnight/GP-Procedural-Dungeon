using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AttachedRoomSpawner : MonoBehaviour
{
    public GameObject thisPrefab;
    //transform.position gives the position of the roomSpawner object, not the prefab
    void Start()
    {
        FloorOverhead overheadSpawner;
        GameObject chosenRoom;
        GameObject spawnedRoom;
        int rotation;
        Vector3 direction;

        overheadSpawner = GameObject.FindObjectOfType<FloorOverhead>();
        chosenRoom = overheadSpawner.RoomPrefabs[UnityEngine.Random.Range(0,5)];

        direction = transform.position - thisPrefab.transform.position;    //gets the direction that the new room will spawn
        direction = new Vector3(Mathf.RoundToInt(direction.x/5)*5, 0, Mathf.RoundToInt(direction.z / 5) *5);

        //all rooms (room 4's middle exit) face -ve z
        //used "5" and "-5" because if I use ">0" or "<0" sometimes the program gives me a ridiculously small number that is close to 0, but not 0, which messes up the code
        //additionally, there should always be a difference of 5
        if (direction.z == 5)
        {
            rotation = 0;
        }
        else if (direction.z == -5)
        {
            rotation = 2;
        }
        else if (direction.x == -5)
        {
            rotation = 3;
        }
        else
        {
            rotation = 1;
        }

        //so 1 exit rooms will start only spawning after 10 rooms have spawned, stopping the program
        if (overheadSpawner.RoomPositions.Count > 10)
        {
            chosenRoom = overheadSpawner.RoomPrefabs[0];
        }

        //2*direction = the length of one room (- or + 10)
        if (overheadSpawner.AddRoom((2*direction) + thisPrefab.transform.position))
        {
            spawnedRoom = Instantiate(chosenRoom, (2*direction) + thisPrefab.transform.position, new Quaternion(0, 0, 0, 0));
            spawnedRoom.transform.Rotate(new Vector3(0, rotation * 90, 0));
        }
    }


    void Update()
    {
    }
}
