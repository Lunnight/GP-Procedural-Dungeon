using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    public GameObject[] Items;
    void Start()
    {
        int randNum;
        GameObject spawnedItem;
        randNum = Random.Range(0, Items.Length * 2);    //*2 so that there's a greater chance no item will spawn

        if (randNum < Items.Length)
        {
            //spawns item at the position of the game object that has this script
            spawnedItem = Instantiate(Items[randNum], new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z), new Quaternion(0, 0, 0, 0));
        }

        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
