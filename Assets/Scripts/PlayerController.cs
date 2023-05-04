using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Data.SqlTypes;

public class PlayerController : MonoBehaviour
{
    
    public Rigidbody rb;
    public GameObject camHolder;
    public float speed, camSense, maxVelocity, jumpVelocity, GAVelocity, superJumpVel;
    private Vector2 move, look, jump, guardianAngel, superJump;
    private float lookRotation;
    private bool grounded, inRange, GAActive;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    
    }


    private void FixedUpdate()
    {
        Move();
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

    public void SetGrounded(bool state)
    {
        grounded = state;
    }

}
