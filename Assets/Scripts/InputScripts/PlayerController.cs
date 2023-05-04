using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Data.SqlTypes;

public class PlayerController : MonoBehaviour
{
    
    public Rigidbody rb;
    public GameObject camHolder;
    public float speed, camSense, maxVelocity;
    private Vector2 move, look;
    private float lookRotation;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();

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

        // limit velocity
        Vector3.ClampMagnitude(velocityChange, maxVelocity);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void LateUpdate()
    {
        // Turn horizontal
        transform.Rotate(Vector3.up * look.x * camSense);

        // Look
        lookRotation += (-look.y * camSense);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);

    }


}
