using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float limitFallVelocity = -2f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchSpeed = 6f;
    
    private float originalHeight;
    private float originalSpeed;

    private Vector3 velocity;

    private bool isGrounded;
    private bool isCrouching = false;

    private void Start()
    {
        originalHeight = controller.height;

        originalSpeed = speed;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = limitFallVelocity;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            
            CheckCrouch();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Guard"))
        {
            //game over
        }

        if (other.CompareTag("WinObject"))
        {
            //return to main menu
        }
    }

    void CheckCrouch()
    {
        if (isCrouching)
        {
            controller.height = crouchHeight;

            speed = crouchSpeed;
        }
        else
        {
            controller.height = originalHeight;

            speed = originalSpeed;
        }
    }
}
