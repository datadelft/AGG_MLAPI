using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class Inventory : NetworkedBehaviour    // make it networked 
{

    public activeWeapon weaponAnchorScript;
    // [0] = Glock
    // [1] = HandCanon
    // [2] = AK


    void Update()
    {


        if (IsLocalPlayer) // only localplayer is swithing weapons
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Debug.Log("Selected Glock");
                weaponAnchorScript.selectedWeapon.Value = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //Debug.Log("Selected Handcanon");
                weaponAnchorScript.selectedWeapon.Value = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //Debug.Log("Selected AK");
                weaponAnchorScript.selectedWeapon.Value = 2;
            }
            
        }

    }
    

}
