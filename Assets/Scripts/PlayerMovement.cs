using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerMovement : NetworkedBehaviour  // use networkedbehavior, that is extending monobehaviour!
{
    //declare vars
    CharacterController cc;
    float pitch = 0f;
    public Transform camTransform;
    public float speed = 5f;
    public float MouseSensitivityX = 3f;
    public float MouseSensitivityY = 3f;
    
    void Start()
    {
        if (!IsLocalPlayer) // If we are not the local player
        {
            camTransform.GetComponent<AudioListener>().enabled = false;
            camTransform.GetComponent<Camera>().enabled = false;
        }
        else // if we are the local player
        {
            cc = GetComponent<CharacterController>(); // get the charactercontroller reference
        }

    }


    private void FixedUpdate()
    {
        if (IsLocalPlayer) // if we are the local player
        {
            movePlayer(); // do movement
            mouseLook();  // do mouse looking
        }
        
    }

    void movePlayer()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // get user input
        move = Vector3.ClampMagnitude(move, 1f); // clamp the value to a maximum of 1, normalising the diagonal movement inputs
        move = transform.TransformDirection(move); // set the direction of the movement to match the transform direction that was set by the mouseLook function
        cc.SimpleMove(move * speed); // do the actual movement trough the character controler

    }

    void mouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivityX; // get horizontal mouse movement with sensitivity
        transform.Rotate(0, mouseX, 0); // rotate the playerObject
        pitch -= Input.GetAxis("Mouse Y") * MouseSensitivityY; // get the vertical mouse movement
        pitch = Mathf.Clamp(pitch, -45f, 45f); // clamp to maximum viewing angle
        camTransform.localRotation = Quaternion.Euler(pitch, 0, 0); // Set the camerapitch accordingly
    }
}
