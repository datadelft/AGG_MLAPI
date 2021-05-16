using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerSetVisibilityLayers : NetworkedBehaviour
{
    // this script hides gameobjects from the localplayer (our own graphic shell)
    // this applies to all children below the 'graphic' gameobject where this script is added


    void Start()
    {
        if (IsLocalPlayer) // if we are not localplayer, set visibility for clones
        {
            //foreach (Transform child in this.transform)
            //    child.gameObject.layer = 9; // layer 8 is localy (and over network?) visible, layer 9 is only visible over the network
            gameObject.layer = 9; // layer 8 is localy (and over network?) visible, layer 9 is only visible over the network
        }

    }

}
