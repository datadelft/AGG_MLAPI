using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;

public class PlayerShooting : NetworkedBehaviour    // make it networked 
{
    public ParticleSystem bulletParticleSystem;
    private ParticleSystem.EmissionModule em;
    NetworkedVarBool shooting = new NetworkedVarBool(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly }, false); // make the bool 'shooting' networked, with writepermission to only owner, and default value of false
    // bool shooting = false;

    float fireRate = 10f;
    float shootTimer = 0f;
    float maxBulletDistance = 100f;
    [SerializeField]
    float bulletDamage = 10f;

    void Start()
    {
        em = bulletParticleSystem.emission; // get the emission control
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            shooting.Value = Input.GetMouseButton(0); // left mouse button down is true/false, feeding into shooting bool. shooting.value needs the value part because this is a networked bool
            shootTimer += Time.deltaTime;
            if (shooting.Value && shootTimer >= 1f/fireRate) // check if we are shooting, and if we are within the timer, we are able to shoot
            {
                shootTimer = 0f; // reset the timer
                //Shoot(); 
                InvokeServerRpc(Shoot); // do shooting on the server trough RPC
            }
        }
        em.rateOverTime = shooting.Value ? fireRate : 0f; // if shooting is true, set rateOvertime to firerate, otherwise to 0f


    }

    [ServerRPC] // run the shooting/taking damage function on the server version of our code. Because all player clones have health as a networkedVarFloat the value of that should cascade down to all connected clients
    void Shoot() 
    {
        Ray ray = new Ray(bulletParticleSystem.transform.position, bulletParticleSystem.transform.forward); // cast a ray from the bulletpoint forward, in the direction of the particles.
        if (Physics.Raycast(ray, out RaycastHit hit, maxBulletDistance))
        {
            // did we hit something and was it a player?
            var player = hit.collider.GetComponent<PlayerHealth>();
            if (player!= null)
            {
                // we did hit a player
                player.TakeDamage(bulletDamage);
            }
        }
    }


}
