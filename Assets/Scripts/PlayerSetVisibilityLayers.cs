using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerSetVisibilityLayers : NetworkedBehaviour
{
    // this script makes networked objects visible for the local player
    


    void Start()
    {
        if (!IsLocalPlayer) // if we are not localplayer, set visibility for clones
        {
            gameObject.layer = 8; // layer 8 is localy visible, makes this object visible for the local player
        }
        
    }

}
