﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocations : MonoBehaviour
{

    public GameObject[] spawnPoint;

    void Start()
    {
        if (spawnPoint.Length == 0)
            spawnPoint = GameObject.FindGameObjectsWithTag("spawnPoint");

        //foreach (GameObject spawn in spawnPoint)
        //{
        //    Instantiate(respawnPrefab, respawn.transform.position, respawn.transform.rotation);
        //}


    }

}
