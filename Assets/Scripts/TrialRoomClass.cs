using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Transactions;
using UnityEngine;

//TO DO: rooms are spawning on top of each other sometimes for some reason
//TO DO: make code to get the correct rotation for stuff to spawn making a clear path from one end of the generation to the other
    //pathway isn't currently working
//TO DO: link rooms
public class TrialRoomClass : MonoBehaviour
{
    Rooms[] floor = new Rooms[17];  //to hold the entire map with reference to each room that is generated
    // if going with when I fill in all the rooms that aren't along the main passage, they are filled in with 1 exit ones then yeah

    public GameObject[] roomPrefabs = new GameObject[5];    // the number of elements that you want in the array goes in the []

    Vector3 spawnPosition;
    Vector3 previousPosition;
    bool validSpawn;

    int roomSize;   //so I know how many coordinates apart each spawn must be
    int currentIndex;   //so I know which part of the array I'm on for adding rooms to the floor array

    //upper and lower bound for generating spawn point
    int lowerBoundGSP;
    int upperBoundGSP;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = new Vector3(0, 0, 0);
        previousPosition = new Vector3(0, 0, 0);
        roomSize = 10;  //has to be 10 otherwise rooms overlap
        currentIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //1 and 4 are defaults, upperBound NOT inclusive (so will +1 later)
        lowerBoundGSP = 1;
        upperBoundGSP = 4;

        validSpawn = false;

        //had to change to <5 as sometimes the current index can go higher than 5
        if (currentIndex < 5)  //so that code doesn't run if the main path has been finished
        {
            //first room spawn onlys
            if (currentIndex == 0)
            {
                floor[currentIndex] = new Rooms(1);
                floor[currentIndex].Position = spawnPosition;
            }

            floor[currentIndex].ChosenPrefab = roomPrefabs[floor[currentIndex].RoomType]; //storing the prefab

            //have to rotate after spawning otherwise rooms won't spawn in correct locations
            Instantiate(floor[currentIndex].ChosenPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
            floor[currentIndex].ChosenPrefab.transform.rotation = new Quaternion(0, floor[currentIndex].returnRotation(), 0, 0);
        }
        else
        {
            Object.Destroy(this);
        }

        currentIndex++;
        previousPosition = spawnPosition;

        //so the next room won't go over the max spawn number of 5 roonms
        if (currentIndex < 5)
        {
            //to make the next room to spawn
            if (currentIndex == 4)
            {
                floor[currentIndex] = new Rooms(1);
            }
            else if (currentIndex != 0)
            {
                floor[currentIndex] = new Rooms(2);
            }
            //for the next room
            floor[currentIndex].Position = spawnPosition;   //doesn't seem to save the correct position of the spawned rooms so am doing this instead

            if (currentIndex > 0)
            {
                if (floor[currentIndex].Rotations == 2)
                {
                    switch (floor[currentIndex].ChosenRotation)
                    {
                        case 1:
                            lowerBoundGSP = 1;
                            upperBoundGSP = 3;
                            break;
                        case 2:
                            lowerBoundGSP = 2;
                            upperBoundGSP = 4;
                            break;
                    }
                }
                else if (floor[currentIndex].Rotations == 4)
                {
                    switch (floor[currentIndex].RoomType)
                    {
                        case 1:
                            lowerBoundGSP = floor[currentIndex].ChosenRotation;
                            upperBoundGSP = floor[currentIndex].ChosenRotation;
                            break;
                        //2 exit L shape
                        case 3:
                            lowerBoundGSP = floor[currentIndex].ChosenRotation;
                            upperBoundGSP = lowerBoundGSP + 1;
                            break;
                        //3 exit
                        case 4:
                            lowerBoundGSP = floor[currentIndex].ChosenRotation;
                            upperBoundGSP = lowerBoundGSP + 2;
                            break;
                    }
                }
            }
            //ensuring one room doesn't spawn on top of another for the next room (not current one)
            //COULD MAKE THIS INTO A METHOD OF THE ROOM CLASS
            while (!validSpawn && (floor[currentIndex].ExitsUsed < floor[currentIndex].NumExits))
            {
                floor[currentIndex].UseExit();

                validSpawn = true;
                spawnPosition = previousPosition;
                GenerateSpawnPosition();

                //checking where the room wants to spawn is clear
                for (int i = 0; i < currentIndex; i++)
                {
                    if (floor[i].Position == spawnPosition)
                    {
                        validSpawn = false;
                    }
                }
            }

            //so that a forever loop won't happen
            if (floor[currentIndex].ExitsUsed == floor[currentIndex].NumExits)
            {
                currentIndex = 5;
            }
        }
    }

    //FUNCTIONS
    Vector3 GenerateSpawnPosition()   //upperBound NOT inclusive, so have to +1
    {
        if (floor[currentIndex].Rotations == 2 || floor[currentIndex].Rotations == -1)  //section works for both 2 rotations and 0 rotations
        {
            switch (Random.Range(lowerBoundGSP, upperBoundGSP + 1))
            {
                case 1:
                    spawnPosition.z += roomSize;
                    break;
                case 2:
                    if (upperBoundGSP != 3) //so if the room is not vertical
                    {
                        spawnPosition.x += roomSize;
                    }
                    break;
                case 3:
                    if (lowerBoundGSP != 2) //if the room is not horizontal
                    {
                        spawnPosition.z -= roomSize;
                    }
                    break;
                case 4:
                    spawnPosition.x -= roomSize;
                    break;
            }

        }
        else if (floor[currentIndex].RoomType == 1 || floor[currentIndex].RoomType == 3)
        {
            switch (Random.Range(lowerBoundGSP, upperBoundGSP + 1))
            {
                case 1:
                case 5:
                    spawnPosition.z += roomSize;
                    break;
                case 2:
                    if (lowerBoundGSP != 1 && upperBoundGSP != 4)
                    {
                        spawnPosition.x += roomSize;
                    }
                    break;
                case 3:
                    if (lowerBoundGSP != 1 && upperBoundGSP != 4)
                    {
                        spawnPosition.z -= roomSize;
                    }
                    break;
                case 4:
                    spawnPosition.x -= roomSize;
                    break;
            }
        }
        else if (floor[currentIndex].RoomType == 4 || floor[currentIndex].RoomType == 5)    //3 and 4 exit rooms
        {
            switch (Random.Range(lowerBoundGSP, upperBoundGSP + 1))
            {
                case 1:
                case 5:
                    spawnPosition.z += roomSize;
                    break;
                case 2:
                case 6:
                    spawnPosition.x += roomSize;
                    break;
                case 3:
                    spawnPosition.z -= roomSize;
                    break;
                case 4:
                    spawnPosition.x -= roomSize;
                    break;
            }
        }
        return spawnPosition;
    }

    //CLASSES
    class Rooms
    {
        int roomType;  //5 different options (check prefabs)
        int rotations;  //the number of ways a room can be rotated
        int chosenRotation; //which rotation out of the number it is

        int exitsUsed;   //on construction will need to declare the amount positions in the array
        int numExits;   //number of exits the room has

        Vector3 position;
        GameObject chosenPrefab;

        public Rooms(int lowerBound)    //lowerBound so I can adjust what rooms can be spawned (default should be 1)
        {
            roomType = Random.Range(lowerBound, 5);

            switch (roomType)
            {
                case 1:
                    numExits = 1;
                    rotations = 4;
                    break;
                case 3:
                    numExits = 2;
                    rotations = 4;
                    break;
                case 4:
                    numExits = 3;
                    rotations = 4;
                    break;
                case 2:
                    numExits = 2;
                    rotations = 2;
                    break;
                default:
                    numExits = 4;
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

            exitsUsed = 0;
        }

        public void UseExit()
        {
            exitsUsed += 1;
        }
        public int ExitsUsed { get => exitsUsed; }
        public int NumExits { get => numExits; }

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
        public int Rotations { get => rotations;}
        public int ChosenRotation { get => chosenRotation;}
    }
}
