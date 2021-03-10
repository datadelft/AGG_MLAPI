using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

public class PlayerHealth : NetworkedBehaviour
{
    public GameObject[] spawnPoints;
    CharacterController cc;
    MeshRenderer[] meshRenderers;

    [SerializeField]
    static float maxHealth = 100f;
    [Header("Health Monitor")]
    public NetworkedVarFloat health = new NetworkedVarFloat(maxHealth); // this is called in playershooting as a serverRPC, the server cascaded this back to us.


     void Start()
    {
        cc = GetComponent<CharacterController>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        spawnPoints = GameObject.FindGameObjectsWithTag("spawnPoint");
    }

    // remember this is running on the server, as it is called trough serverRPC
    public void TakeDamage(float damage)
    {
        health.Value -= damage;
        // chech health state 
        if (health.Value <= 0) // we're death,
        {
            // do death anim (trough rpc i gues)

            // reset health
            health.Value = maxHealth;

            // respawn (trough client rpc)
            int index = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[index].transform.position; // set respawn location to the position of a random spawnpoint gameobject
            InvokeClientRpcOnEveryone(ClientRespawn, spawnPos);
        }
    }

    [ClientRPC]
    void ClientRespawn(Vector3 position)
    {
        
        StartCoroutine(Respawn(position));
    }

    IEnumerator Respawn(Vector3 position)
    {
        // make the player invisible
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = false;
        }

        // disable shooting script and movement script
        //

        // delay
        yield return new WaitForSeconds(1f);

        // disable charactercontroller
        cc.enabled = false;

        // set position to the chosen respawn position
        transform.position = position;

        // make the player visible
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = true;
        }

        // enable shooting script and movement script
        //

        // enable charactercontroller
        cc.enabled = true;
    }


}
