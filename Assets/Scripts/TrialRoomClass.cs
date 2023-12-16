using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//TO DO: make code to get the correct rotation for stuff to spawn making a clear path from one end of the generation to the other
public class TrialRoomClass : MonoBehaviour
{
    Rooms[] floor = new Rooms[17];  //to hold the entire map with reference to each room that is generated
    // if going with when I fill in all the rooms that aren't along the main passage, they are filled in with 1 exit ones then yeah

    string[] roomLinks;    //holds the pointer of the rooms that are linked, multiple links are separated by '#'
    public GameObject[] roomPrefabs = new GameObject[5];    // the number of elements that you want in the array goes in the []

    Vector3 spawnPosition;
    Vector3 previousPosition;
    bool validSpawn;

    int roomSize;   //so I know how many coordinates apart each spawn must be
    int currentIndex;   //so I know which part of the array I'm on for adding rooms to the floor array

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = new Vector3(0, 0, 0);
        previousPosition = new Vector3(0, 0, 0);
        roomSize = 10;  //has to be 10 otherwise rooms overlap
        currentIndex = 0;
        validSpawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        validSpawn = false;

        floor[currentIndex] = new Rooms();
        floor[currentIndex].ChosenPrefab = (roomPrefabs[floor[currentIndex].RoomType]); //storing the prefab

        //have to rotate after spawning otherwise rooms won't spawn in correct locations
        Instantiate(floor[currentIndex].ChosenPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
        floor[currentIndex].Position = spawnPosition;   //doesn't seem to save the correct position of the spawned rooms so am doing this instead
        floor[currentIndex].ChosenPrefab.transform.rotation = new Quaternion(0, floor[currentIndex].returnRotation(),0,0);

        currentIndex++;
        previousPosition = spawnPosition;
        
        //ensuring one room doesn't spawn on top of another
        while (!validSpawn)
        {
            validSpawn = true;
            spawnPosition = previousPosition;
            GenerateSpawnPosition();

            //checking where the room wants to spawn is clear
            for (int i = 0; i < currentIndex; i++)
            {
                if (floor[i].Position == spawnPosition)   //currentIndex - 1 because we've already increased the index by 1
                {
                    validSpawn = false;
                    Debug.Log(validSpawn);
                }
            }
        }

        if (currentIndex == 5)

        {
            Object.Destroy(this);
        }
    }

    Vector3 GenerateSpawnPosition()
    {
        switch (Random.Range(1, 5))
        {
            case 1:
                spawnPosition.z += roomSize;
                break;
            case 2:
                spawnPosition.x += roomSize;
                break;
            case 3:
                spawnPosition.z -= roomSize;
                break;
            case 4:
                spawnPosition.x -= roomSize;
                break;
        }
        return spawnPosition;
    }
    class Rooms
    {
        int roomType;  //5 different options (check prefabs)
        int rotations;  //the number of ways a room can be rotated
        int chosenRotation; //which rotation out of the number it is
        bool[] exitsUsed;   //on construction will need to declare the amount positions in the array
        Vector3 position;
        GameObject chosenPrefab;

        public Rooms()
        {
            roomType = Random.Range(1, 5);

            switch (roomType)
            {
                case 1:
                case 3:
                case 4:
                    rotations = 4;
                    break;
                case 2:
                    rotations = 2;
                    break;
                default:
                    rotations = -1; //doesn't have multiple rotations
                    break;
            }

            if (rotations != -1)
            {
                chosenRotation = Random.Range(0, rotations);
            }
            else
            {
                chosenRotation = -1;    //no specific rotation because none available
            }

            if (roomType > 2)
            {
                exitsUsed = new bool[roomType - 1]; //so that room has the correct number of exits
                //how I've designed this means that this idea should work
            }
            else
            {
                exitsUsed = new bool[roomType];
            }

            //ensuring none of the exits have been used yet
            for (int x = 0; x < exitsUsed.Length; x++)
            {
                exitsUsed[x] = false;
            }
        }

        public void UseExit(int exit)
        {
            exitsUsed[exit - 1] = true;
        }

        public GameObject ChosenPrefab {get => chosenPrefab; set => chosenPrefab = value; }

        public int RoomType { get => roomType; }
        public int returnRotation()
        {
            int degrees = 0;    //set to 0 so thing doesn't complain
            switch (rotations)
            {
                case 2:
                case 4:
                    degrees = (chosenRotation - 1) * 90;    //to rotate the room correctly on spawn
                    break;
                default:
                    degrees = 0;
                    break;
            }
            return degrees;
        }
        public Vector3 Position { get => position; set => position = value; }
    }
}
