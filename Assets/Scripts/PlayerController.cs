using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Data.SqlTypes;

public class PlayerController : MonoBehaviour
{
    // GameObject & components
    public Rigidbody rb;
    public GameObject camHolder;
    public GameObject ally;
    private MercyInput input;

    // Base movement variables
    public float speed, camSense, maxVelocity, jumpVelocity;
    private Vector2 move, look;
    private float lookRotation;
    private bool grounded;

    // Mercy mechanic variables
    public float GASpeed, SuperJumpSpeed;
    public float GAEndThreshold = 0.5f;
    public float allyInViewThreshold = 30f;
    public float allyInRangeThreshold = 10f;
    private bool superJumpActive = false;
    private bool GAActive = false;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        input = new MercyInput();

        // Subscribe (+=) OnGuardianAngel to the 'performed' event. OnGuardianAngel method
        // is triggered when the GuardianAngel action key ('context') is pressed/performed
            // Same for SuperJump
        input.Player.GuardianAngel.performed += context => OnGuardianAngel(context);
        input.Player.SuperJump.performed += context => OnSuperJump(context);
        Debug.Log("GAEndThreshold = " + GAEndThreshold);

    }


    private void FixedUpdate()
    {
        Move();
        //CheckGACond();
        //Debug.Log("GA Status: " + GAActive);

        // Check if player is currently flying or Super Jumping
        if (GAActive)
        {
            CheckAllyReached();
        }
        else if (superJumpActive)
        {
            CheckSuperJump();
        }
    }

    void LateUpdate()
    {
        Look();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void OnGuardianAngel(InputAction.CallbackContext context)
    {
        if (!GAActive && allyInFOV() && allyInRange() && context.performed)
        {
                StartCoroutine(ActivateGASJ());
                Debug.Log("GA Status: " + GAActive);
        }        
    }

    public void OnSuperJump(InputAction.CallbackContext context)
    { 
        if (GAActive)
        {
            superJumpActive = true;
        }
    }

    void Move()
    {
        // find new velocity
        Vector3 currentVelocity = rb.velocity;
        Vector3 newVelocity = new Vector3(move.x, 0, move.y);
        newVelocity *= speed;

        // velocity direction
        newVelocity = transform.TransformDirection(newVelocity);

        // calculate change in velocity
        Vector3 velocityChange = newVelocity - currentVelocity;

        // MAY NEED TO CHANGE TO GET GA/SUPER JUMP TO WORK
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        // limit velocity
        Vector3.ClampMagnitude(velocityChange, maxVelocity);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Look()
    {
        // Turn horizontal
        transform.Rotate(Vector3.up * look.x * camSense);

        // Look
        lookRotation += (-look.y * camSense);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    void Jump()
    {
        Vector3 jumpVelocityCurr = Vector3.zero;

        if (grounded)
        {
            jumpVelocityCurr = Vector3.up * jumpVelocity;
        }

        rb.AddForce(jumpVelocityCurr, ForceMode.VelocityChange);
    }

    // used to determine the state of the player, in relation to being on the ground
        // if the player is touching the ground, this allows them to jump
    public void SetGrounded(bool state)
    {
        grounded = state;
    }

    // Coroutine for Guardian Angel/Super Jump ability activation
    IEnumerator ActivateGASJ()
    {
        GAActive = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = ally.transform.position;

        float GADistance = Vector3.Distance(startPos, endPos);
        float GADuration = GADistance / GASpeed;
        float GAFlyTime = 0f;

        Debug.Log("GADistance = " + GADistance);

        while (GAFlyTime < GADuration && !superJumpActive)
        {
            rb.MovePosition(Vector3.Lerp(startPos, endPos, GAFlyTime / GADuration));
            GAFlyTime += Time.deltaTime;
            yield return null;
        }

        if (superJumpActive)
        {
            rb.AddForce(Vector3.up * SuperJumpSpeed, ForceMode.VelocityChange);
        }
        else
        {
            rb.MovePosition(endPos);
        }

        GAActive = false;
        superJumpActive = false;
    }

    // Check for ally's position in player's FOV
    bool allyInFOV()
    {
        Vector3 allyDirection = ally.transform.position - transform.position;
        float allyPosAngle = Vector3.Angle(transform.forward, allyDirection);
        //Debug.Log("ally in FOV");
        return allyPosAngle <= allyInViewThreshold;
    }

    // Check if ally is within range for player to use GA
    bool allyInRange()
    {
        return Vector3.Distance(transform.position, ally.transform.position) <= allyInRangeThreshold;
    }

    // check to see if the player has reached their ally
    void CheckAllyReached()
    {
        if (Vector3.Distance(transform.position, ally.transform.position) < GAEndThreshold)
        { 
            GAActive = false;
            //rb.velocity = Vector3.zero;
            Debug.Log("GAEndthreshold reached");
        }
    }

    // check to see if player is using SuperJump
    void CheckSuperJump()
    {
        if (rb.velocity.y <= 0f)
        {
            superJumpActive = false;
        }
    }








    // print to console when GA is able to be activated
    void CheckGACond()
    {
        if (allyInRange() && allyInFOV())
        {
            Debug.Log("GA Conditions Met!");
        }
    }


}
