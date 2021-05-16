using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI; // use networking 
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

public class PlayerHealth : NetworkedBehaviour // use networkedbehavior, that is extending monobehaviour!
{
    private SpawnLocations spawnPointLocationsScript;
    CharacterController cc;
    MeshRenderer[] meshRenderers;
    SkinnedMeshRenderer[] skinnedMeshRenderers;
    private AudioSource playerAudio;

    [SerializeField]
    static float maxHealth = 100f;
    [Header("Health Monitor")]
    public NetworkedVarFloat health = new NetworkedVarFloat(maxHealth); // this is called in playershooting as a serverRPC, the server cascaded this back to us.


     void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        cc = GetComponent<CharacterController>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        spawnPointLocationsScript = GameObject.FindGameObjectWithTag("spawnPointManager").GetComponent<SpawnLocations>();
    }

    // remember this is running on the server, as it is called trough serverRPC
    public void TakeDamage(float damage)
    {
        health.Value -= damage;
        // chech health state 
        if (health.Value <= 0) // we're death,
        {
            // do death anim (trough rpc i guess, to sync over the network to other clients)
            //
            //

            // do au or death sound
            //playerAudio.PlayOneShot(clip);

            // reset health 
            health.Value = maxHealth;

            // check our list of renderers, this could have been changed due to inventory changes
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();


            // respawn (trough client rpc)
            Vector3 spawnPos = Vector3.zero; // if no spawnpoints found, send vector3.zero as alternate spawnpoint
            if (spawnPointLocationsScript && spawnPointLocationsScript.spawnPoint.Length != 0) // if spawnpoints are registered in the map, return a random one. If not, use vector3.zero as spawn.
            {
                int index = Random.Range(0, spawnPointLocationsScript.spawnPoint.Length);
                spawnPos = spawnPointLocationsScript.spawnPoint[index].transform.position; // set respawn location to the position of a random spawnpoint gameobject
            }
            InvokeClientRpcOnEveryone(ClientRespawn, spawnPos);
        }
    }

    [ClientRPC] // distrubute over the network
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
            //also disable the hitbox/colliders?
        }
        foreach (var renderer in skinnedMeshRenderers)
        {
            renderer.enabled = false;
            //also disable the hitbox/colliders?
        }

        // disable shooting script and movement script
        //
        // 2do

        // disable charactercontroller
        cc.enabled = false;

        // delay
        yield return new WaitForSeconds(1f);

        // set position to the chosen respawn position
        transform.position = position;

        // make the player visible
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = true;
        }
        foreach (var renderer in skinnedMeshRenderers)
        {
            renderer.enabled = true;
        }

        // enable shooting script and movement script
        //
        // 2do

        // enable charactercontroller
        cc.enabled = true;
    }


}
