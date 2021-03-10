using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerMovement : NetworkedBehaviour  // use networkedbehavior, that is extending monobehaviour!
{
    //declare vars
    CharacterController cc;
    public float speed = 5f;
    public float MouseSensitivityX = 3f;
    public float MouseSensitivityY = 3f;

    void Start()
    {
        cc = GetComponent<CharacterController>(); // get the charactercontroller reference
    }


    private void FixedUpdate()
    {
        if (!IsLocalPlayer) // If we are not the local player
        { 

        }
        else // if we are the local player
        {
            movePlayer();
        }
        
    }

    void movePlayer()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // get user input
        move = Vector3.ClampMagnitude(move, 1f); // clamp the value to a maximum of 1, normalising the diagonal movement inputs
        move = transform
        cc.SimpleMove(move * speed); // do the actual movement

    }

    void mouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivityX;
        transform.Rotate(0, mouseX, 0);

    }
}
