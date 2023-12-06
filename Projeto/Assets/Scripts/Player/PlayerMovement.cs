using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação")]
    public bool isSprinting = false;
    public float sprintSpeed = 17f;
    float currentSpeed;
    public float gravity = -9.81f;
    public CharacterController characterController;

    [Header("Pulo")]
    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public Transform groundCheck;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    public int jumpsRemaining = 1;
    //public GameObject doubleJumpEffect;

    [Header("Agachar")]
    public Transform playerBody;
    public bool isCrouching = false;
    Vector3 crouchScale = new Vector3(1.2f, 0.9f, 1.2f);
    Vector3 playerScale = new Vector3(1.2f, 1.8f, 1.2f);

    [Header("Dash")]
    public float dashSpeed = 10f;
    public float dashDistance = 5f;
    public float dashCooldown = 3f;
    bool isDashing = false;
    float dashCooldownTimer;

    [Header("Outros")]
    public PlayerStats playerStats;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpsRemaining = 1;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * currentSpeed * Time.deltaTime);

        //PULAR
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpsRemaining--;
                //GameObject doubleJumpPrefab = Instantiate(doubleJumpEffect, groundCheck);
                //Destroy(doubleJumpPrefab, 1f);
            }
        }

        //CORRER
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            isSprinting = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
            currentSpeed = playerStats.speed;
        }

        //AGACHAR (ARRUMAR)
        /*if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isCrouching)
        {
            isCrouching = true;
            playerBody.transform.localScale = crouchScale;
            characterController.transform.localScale = crouchScale;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) && isGrounded && isCrouching)
        {
            isCrouching = false;
            playerBody.transform.localScale = playerScale;
            characterController.transform.localScale = playerScale;
        }*/

        //AVANÇAR
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        /*if (Input.GetKeyDown(KeyCode.Q) && pistolOnHand)
        {
            machinegunOnHand = true;
            machinegun.SetActive(true);
            pistolOnHand = false;
            pistol.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && machinegunOnHand)
        {
            machinegunOnHand = false;
            machinegun.SetActive(false);
            pistolOnHand = true;
            pistol.SetActive(true);
        }*/
    }

    IEnumerator Dash()
    {
        isDashing = true;

        Vector3 dashDirection = characterController.velocity.normalized;
        float dashTimer = 0;

        while (dashTimer < 0.5f) // Tempo total do dash
        {
            characterController.Move(dashDirection * dashDistance * Time.deltaTime * dashSpeed); // Ajuste o multiplicador para a velocidade desejada

            dashTimer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        dashCooldownTimer = dashCooldown;
    }
}
