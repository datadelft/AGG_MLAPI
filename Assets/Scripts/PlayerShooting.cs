using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;
using MLAPI.Spawning;
public class PlayerShooting : NetworkedBehaviour    // make it networked 
{

    NetworkedVarBool shooting = new NetworkedVarBool(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly }, false); // make the bool 'shooting' networked, with writepermission to only owner, and default value of false
    float shootTimer = 0f;
    public GameObject weaponAnchor;
    private GameObject rfp; // riffle firing point

    [SerializeField]
    private GameObject[] bulletImpactPrefab;
    // bulletImpactPrefab[0] = Concrete no collision
    // bulletImpactPrefab[1] = Concrete
    // bulletImpactPrefab[2] = Dirt
    // bulletImpactPrefab[3] = Metal
    // bulletImpactPrefab[4] = Sand
    // bulletImpactPrefab[5] = Softbody
    // bulletImpactPrefab[6] = Wood no collision
    // bulletImpactPrefab[7] = Wood
    [SerializeField] 
    private GameObject bulletholePrefab;
    [SerializeField]
    private AudioClip hitmarkerSnd;
    [SerializeField]
    private AudioClip[] weaponSound;
    // 0 = glock
    // 1 = handcanon
    // 2 = AK47
    private AudioSource weaponAudio;
    private AudioSource playerAudio;
    // gun specific. Change these on WeaponSwitch
    private ParticleSystem ps;
    private ParticleSystem sparks;
    private ParticleSystem.EmissionModule em; // the emission controller for the muzzleflash
    public float fireRate = 10f;
    public float maxBulletDistance = 100f;
    public float bulletDamage = 10f;
    public int numberOfBulletsPerShot = 1;
    public float bulletSpread = 0f;
    public int weaponSoundNr;
    [SerializeField]
    float difficultySpread = 0f;

    void Start()
    {
        weaponAudio = weaponAnchor.GetComponent<AudioSource>();
        //Debug.Log(weaponAudio);
    }

    // Update is called once per frame
    void Update()
    {

        if (IsLocalPlayer)
        {
            shooting.Value = Input.GetMouseButton(0); // left mouse button down is true/false, feeding into shooting bool. shooting.value needs the value part because this is a networked bool
            shootTimer += Time.deltaTime;
            if (shooting.Value && shootTimer >= 1f / fireRate) // check if we are shooting, and if we are within the timer, we are able to shoot
            {
                shootTimer = 0f; // reset the timer
                if (rfp) // if we have a gun (and thus a riffle firing point)
                {
                    InvokeServerRpc(Shoot); // do shooting on the server trough RPC
                }
            }

        }
        if (rfp) // presume that we found em if we got the rfp
            em.rateOverTime = shooting.Value ? fireRate : 0f; // if shooting is true, set rateOvertime to firerate, otherwise to 0f
    }

    [ServerRPC] // run the shooting/taking damage function on the server version of our code. Because all player clones have health as a networkedVarFloat the value of that should cascade down to all connected clients
    void Shoot()
    {
        if (ps)
        {
            ps.Emit(1); // emit one flash
            ps.Play();
            ps.Stop();
        }
        if (sparks)
        {
            sparks.Emit(1); // emit one flash
            sparks.Play();
            sparks.Stop();
        }
        if (weaponAudio)
            weaponAudio.PlayOneShot(weaponSound[weaponSoundNr]);

        for (int i = 0; i < numberOfBulletsPerShot; i++) // check if we use a single round weapon or are we firing multiple rounds at once live the handcannon?
        { 
            Vector3 direction = rfp.transform.forward; // Bullet Spread, raycast will take the spread into account
            float actualSpread = bulletSpread + difficultySpread;
            direction.x += Random.Range(-actualSpread, actualSpread);
            direction.y += Random.Range(-actualSpread, actualSpread);
            direction.z += Random.Range(-actualSpread, actualSpread);

            Ray ray = new Ray(rfp.transform.position, direction); // cast a ray from the rifle firing point forward in the looking direction
            if (Physics.Raycast(ray, out RaycastHit hit, maxBulletDistance))
            {
                //Debug.Log("We hit " + hit.transform.name);
                // did we hit something and was it a player?
                var player = hit.collider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    // we did hit a player
                    player.TakeDamage(bulletDamage);
                    // do blood splat effect
                    GameObject bulletImpact = Instantiate(bulletImpactPrefab[5], hit.point, Quaternion.LookRotation(hit.normal));
                    bulletImpact.GetComponent<NetworkedObject>().Spawn(); // have the server spawn this on the network
                    // do hitmarker sound
                    if (playerAudio)
                        playerAudio.PlayOneShot(hitmarkerSnd);
                }
                else
                {
                    // we hit something else, do impact sim with spawning bulletImpactPrefab[]
                    // When a spawned object gets destroyed on the server/host, MLAPI will automatically destroy it on all clients as well.
                    GameObject bulletImpact = Instantiate(bulletImpactPrefab[2], hit.point, Quaternion.LookRotation(hit.normal));
                    bulletImpact.GetComponent<NetworkedObject>().Spawn(); // have the server spawn this on the network
                    GameObject bulletHole = Instantiate(bulletholePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    bulletHole.GetComponent<NetworkedObject>().Spawn(); // have the server spawn this on the network
                }
            }
        }
    }


    public void changedWeapon()
    {
        //Debug.Log("Reinitialising shootingscript");
        // player changed weapon, find new riffle firing point and emission controller.
        Transform[] children = weaponAnchor.transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == "RifleFirePoint")
            {
                rfp = child.gameObject;
            }
        }
        //Debug.Log("We found RFP: " + rfp.name);
        if (!rfp)
            Debug.LogError("RifleFirePoint (exact name) is missing from weapon prefab!");
        ps = rfp.GetComponentInChildren<ParticleSystem>();
        ps.Stop();
        sparks = ps.transform.Find("Sparks").GetComponentInChildren<ParticleSystem>();
        sparks.Stop();
        //Debug.Log("We found sparks at " + sparks.transform.name);
        em = ps.emission; // get the emission control
        shootTimer = 1f / fireRate + 1f; // gun is ready to fire, timer is higher than treshold
    }
}



