using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;
using MLAPI.Spawning;

//this will ask all the clients to swap if you are the server, or the sewrver to swap if you are a client
//RpcSwap will only call localSwap.Making the clients that receive the rpc to update
//CmdSwap will call localSwap and RpcSwap.Making the host update the changes and distribute them.
//On top of RpcSwap ask if(isLocalPlayer) and return (because you are receiving back the notification that you started)


public class activeWeapon : NetworkedBehaviour    // make it networked 
{
    // 0 = glock
    // 1 = handcanon
    // 2 = AK
    [SerializeField]
    private GameObject[] weaponSlot;
    [SerializeField] 
    private float[] fireRate; // higher is faster
    [SerializeField] 
    private float[] maxBulletDistance;
    [SerializeField] 
    private float[] bulletDamage;
    [SerializeField] 
    private int[] numberOfBulletsPerShot; // int
    [SerializeField] 
    private float[] bulletSpread;
    
    //public int selectedWeapon = 0;
    public NetworkedVarInt selectedWeapon = new NetworkedVarInt(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly }, 0); // make the int 'selectedWeaponNet' networked, with writepermission to only owner, and default value of false
    public PlayerShooting plShootScript;
    [SerializeField]
    private snapToGun snapScript;

    private int previousWeapon = -1;
    public bool leftHandLocked = false;
    public bool rightHandLocked = false;

    void Update()
    {
        //if (!IsLocalPlayer)
        //    return;
            

        if (selectedWeapon.Value != previousWeapon) // Did the weaponselection change?
        {
            //Debug.Log("Weapon change request caught");
            if (selectedWeapon.Value >= 0 && selectedWeapon.Value <= weaponSlot.Length) // validate the input, is the requested weapon ID in range?
            {
                //Debug.Log("Weapon change request validated");
                previousWeapon = selectedWeapon.Value; // reset so we will run this code only once per change

                // This is taken from https://answers.unity.com/questions/1414490/set-active-over-network.html and translated to MLAPI
                if (IsLocalPlayer)
                {
                    leftHandLocked = false;
                    rightHandLocked = false;
                    localSwapWeapon();
                }

                if (IsServer)   // now we check if we are also the server (aka, the host)
                {
                    // we are the host, so we just have to tell all connected clients we changed our weapon and also do our local weapon swap
                    serverRpcSwapWeapon();
                    //InvokeServerRpc(serverRpcSwapWeapon); // this is used for asking the server to do the serverRPC for us if we are not server ourselves???
                }
                else
                {
                    // we are NOT the host, so we ask the server/host to send the message that we are swapping our weapon to all connected clients for us
                    //InvokeServerRpc(serverRpcSwapWeapon); // Ask the server to send the word out to our fellow dudes!
                    clientRpcSwapWeapon();
                }
            }
            else // we received invalid input
            {
                selectedWeapon.Value = previousWeapon; // invalid input caught, reset requestedweapon to previous weapon to prevent indefinite looping 
            }
        }

    }

    void localSwapWeapon() 
    {
        //
        // main weapon switch procedure
        //

        //Debug.Log("Request validated, disabling all weapons prior to change");
        for (int i = 0; i < weaponSlot.Length; i++) // loop trough the weaponslots disabling them all
        {
            weaponSlot[i].SetActive(false);
        }
        //Debug.Log("Enable the requested weapon " + weaponSlot[selectedWeapon].name);
        // enable the selected weapon and set the config values accordingly
        weaponSlot[selectedWeapon.Value].SetActive(true);
        plShootScript.fireRate = fireRate[selectedWeapon.Value];
        plShootScript.maxBulletDistance = maxBulletDistance[selectedWeapon.Value];
        plShootScript.bulletDamage = bulletDamage[selectedWeapon.Value];
        plShootScript.numberOfBulletsPerShot = numberOfBulletsPerShot[selectedWeapon.Value];
        plShootScript.bulletSpread = bulletSpread[selectedWeapon.Value];
        plShootScript.weaponSoundNr = selectedWeapon.Value;
        plShootScript.changedWeapon(); // reinitialise the shootingscript
        int myHash = this.transform.root.GetComponent<NetworkedObject>().GetHashCode();
        //Debug.Log("Changing weapon on " + myHash + " completed");
    }

    [ServerRPC]
    void serverRpcSwapWeapon()
    {
        // if we are the server, doing the swap will also tell the network that we did so.
        // it will send this RPC to all the the clones for localplayer on the other connected clients
        localSwapWeapon();
    }

    [ClientRPC]
    void clientRpcSwapWeapon()
    {
        // if we are the localplayer, the swap already happend for us locally. We ask the server to send out and RPC call to all connected clients
        // to have our clones on their version of the world to swap weapons too.


        localSwapWeapon();
        
    }





}
