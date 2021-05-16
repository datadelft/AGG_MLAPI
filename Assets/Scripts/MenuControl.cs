using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI; // use networking
using MLAPI.Spawning;
using System;

public class MenuControl : MonoBehaviour
{
    public GameObject gameMenuPanel;
    [SerializeField]
    private string connectionPassword = "FragLeonPlx";
    private SpawnLocations spawnPointLocationsScript;

    private void Start()
    {
        spawnPointLocationsScript = GameObject.FindGameObjectWithTag("spawnPointManager").GetComponent<SpawnLocations>();
    }

    Vector3 GetRandomSpawn()
    {
        if (spawnPointLocationsScript && spawnPointLocationsScript.spawnPoint.Length != 0) // if no spawnpointmanager is found or the manager didn't find any spawnpoints, return alternative set spawnpoint
        {
            // retrieve a random spawnpoint gameobject reference from the spawnpoint locations manager's array. Return the vector3 position as the current spawnpoint
            int index = UnityEngine.Random.Range(0, spawnPointLocationsScript.spawnPoint.Length);
            return spawnPointLocationsScript.spawnPoint[index].transform.position;
        }
        else
        {
            return Vector3.zero; // alternative spawnpoint if no spawnpoints were found
        }
    }


    // This happens on the server
    public void Host()
    {
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkingManager.Singleton.StartHost(GetRandomSpawn(), Quaternion.identity); // can set spawn location if needed
        gameMenuPanel.SetActive(false);
    }
    // This happens on the server
    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkingManager.ConnectionApprovedDelegate callback)
    {
        //check incomming data
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == connectionPassword + Application.version; // decode our connectionPassword, return true of it matches.
        Debug.Log("Approving the connection on gameclient version " + Application.version + " for " + clientID);
        callback(true, null, approve, GetRandomSpawn(), Quaternion.identity);
    }

    public void Join()
    {
        NetworkingManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(connectionPassword + Application.version); // encode our connectionPassword. this is the "password" for the connection
        NetworkingManager.Singleton.StartClient();
        gameMenuPanel.SetActive(false);
    }
}
