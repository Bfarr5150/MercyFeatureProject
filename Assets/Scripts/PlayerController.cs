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
    private bool GAActive = false;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("GAActive state: " + GAActive);
        Debug.Log("Ally in view: " + allyInFOV());

    }


    private void FixedUpdate()
    {
        Move();
        if (allyInFOV() == true)
        {
            Debug.Log("Ally is in view");
        }
        if (allyInRange() == true)
        {
            Debug.Log("Ally is in range");
        }
        /*
        if (!GAActive)
        {
            if(allyInRange() && allyInFOV())
            {
                ActivateGA();
                Debug.Log("GA activated");
            }
        }
        */
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
                StartCoroutine(ActivateGA());
                Debug.Log("GA activated");
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

    // Coroutine for Guardian Angel ability activation
    IEnumerator ActivateGA()
    {
        GAActive = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = ally.transform.position;

        float GADistance = Vector3.Distance(startPos, endPos);
        float GADuration = GADistance / GASpeed;

        float GAFlyTime = 0f;
        while (GAFlyTime < GADuration)
        {
            rb.MovePosition(Vector3.Lerp(startPos, endPos, GAFlyTime / GADuration));
            GAFlyTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(endPos);

        GAActive = false;
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


}
