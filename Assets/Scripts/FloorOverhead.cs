using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorOverhead : MonoBehaviour
{
    public List<Vector3> RoomPositions; //"List" is a dynamic array essentially
    public GameObject[] RoomPrefabs = new GameObject[5];

    // Start is called before the first frame update
    void Start()
    {
        GameObject startingRoom;
        RoomPositions = new List<Vector3>();

        //instantiates a random starting room
        startingRoom =  Instantiate(RoomPrefabs[UnityEngine.Random.Range(0, 1)],new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        startingRoom.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(0, 4) * 90, 0));  //gives room random rotation

        RoomPositions.Add(new Vector3(0,0,0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddRoom(Vector3 roomPosition)   //only function that needs to be accessed by the blank room spawners
    {
        bool available;

        available = CheckFree(roomPosition);

        if (available)
        {
            RoomPositions.Add(roomPosition);
        }
        return available;
    }

    bool CheckFree(Vector3 newRoomPosition)
    {
        bool available = true;

        foreach (Vector3 roomPosition in RoomPositions)
        {
            if (roomPosition == newRoomPosition)
            {
                available = false;
            }
        }
        return available;
    }
}
