using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI; // use networking


public class PlayerMovement : NetworkedBehaviour  // use networkedbehavior, that is extending monobehaviour!
{
    //declare vars
    CharacterController cc;
    public float walkingSpeed = 6f;
    public float runningSpeed = 12f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    Vector3 moveDirection = Vector3.zero;
    float pitch = 0f;
    public float MouseSensitivityX = 3f;
    public float MouseSensitivityY = 3f;
    public float maxPitch = 45f;
    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private Animator anim = null;

    float verticalSpeed = 0f;
    float previousJumpVelocity = 0f;



    void Start()
    {
        
        if (!IsLocalPlayer) // If we are not the local player
        {
            playerCamera.GetComponent<AudioListener>().enabled = false; // we don't want to echo
            playerCamera.GetComponent<Camera>().enabled = false; // we don't want multiple camera's being active in the scene 
        }
        else // if we are the local player
        {
            cc = GetComponent<CharacterController>(); // get the charactercontroller reference
            //cc = GetComponent<CharacterController>(); 
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // get user input
        //move = Vector3.ClampMagnitude(move, 1f); // clamp the value to a maximum of 1, normalising the diagonal movement inputs
        //move = transform.TransformDirection(move); // set the direction of the movement to match the transform direction that was set by the mouseLook function
        //cc.SimpleMove(move * speed); // do the actual movement trough the character controller

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && cc.isGrounded)
        {
            moveDirection.y = jumpSpeed;
            previousJumpVelocity = 0f;
            anim.SetBool("jump", true);
            //Debug.Log("Jumping");
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!cc.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        verticalSpeed = cc.velocity.y - previousJumpVelocity;
        if (cc.velocity.y < previousJumpVelocity) // character falling or going down
        {
            //Debug.Log("falling with speed " + verticalSpeed);
            if (verticalSpeed >= -0.5f) // fallingspeed is 0, we landed!
            {
                //Debug.Log("Landed");
                anim.SetBool("jump", false);
            }
        }
        if (cc.velocity.y > previousJumpVelocity) // character rising up, climbing or jumping upwards
        {
            //Debug.Log("rising");
        }


        // Move the controller
        cc.Move(moveDirection * Time.deltaTime);

        //
        //
        // do the animations
        //
        // 2do:
        // use velocity in a blend tree for walking anims. 
        // add jumping anim.
        // 
        //
        //
        // use this for idle/walk/run anim blendtree
        //Debug.Log("Velocity = " + cc.velocity.magnitude + "And rotation is " + cc.transform.localRotation);
        //Debug.Log("X = " + curSpeedX / runningSpeed + " / Y = " + curSpeedY / runningSpeed);
        // Update the Animator with our values so that the blend tree updates
        anim.SetFloat("velocityX", (curSpeedX / runningSpeed));
        anim.SetFloat("velocityY", (curSpeedY / runningSpeed));

        //if (cc.velocity != Vector3.zero) // we are moving and have an animator component 
        //{
        //    anim.SetBool("isWalking", true);
        //}
        //else // we are not moving 
        //{
        //    anim.SetBool("isWalking", false);

        //}

        //
        // end of anims
        // 


    }

    void mouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivityX; // get horizontal mouse movement with sensitivity
        transform.Rotate(0, mouseX, 0); // rotate the playerObject
        pitch -= Input.GetAxis("Mouse Y") * MouseSensitivityY; // get the vertical mouse movement
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch); // clamp to maximum viewing angle
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0); // Set the camerapitch accordingly
    }
}
