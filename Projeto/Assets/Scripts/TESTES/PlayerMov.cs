using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    [Header("Estado")]
    public MovementState state;

    [Header("Movimento")]
    public float groundDrag;
    public float jumpForce;
    public float airMultiplier;
    [Space]
    public float crouchYScale;
    float startYScale;
    [Space]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    bool exitingSlope;
    [Space]
    public float moveSpeed; //Velocidade atual do jogador
    public float walkSpeed;
    public float sprintSpeed;
    public float crouchSpeed;

    [Header("Teclas e Atalhos")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Refer�ncias e Configura��es")]
    public PlayerStats playerStats;
    public Transform orientation;
    bool readyToJump; //Controle do pulo

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask groundMask;
    bool isGrounded; //Detec��o de ch�o

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection; //Vetor que armazena a dire��o de movimento
    Rigidbody rb; //Rigidbody do jogador


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //Checa se o jogador est� no ch�o
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        MyInput(); //Entradas do jogador
        SpeedControl(); //Controle de velocidade
        StateHandler(); //Controla estado de movimento do jogador, como andando, correndo, agachado, etc

        //Evita deslizamento no ch�o
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Pular
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump(); //Executa o pulo
            Invoke(nameof(ResetJump), 0.25f);
        }

        //Agachar
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //Se soltar bot�o, levantar
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }



    //MOVIMENTO
    private void MovePlayer()
    {
        //Calcula a dire��o de movimento
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope) //Na rampa
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //Desliga gravidade quando na rampa
        rb.useGravity = !OnSlope();

        if (isGrounded) //No ch�o
        {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        }
        else if (!isGrounded) //No ar
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        //Limita a velocidade em uma rampa
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        //Limita a velocidade se no ch�o ou no ar
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //Limita a velocidade se necess�rio
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }



    //PULO
    private void Jump()
    {
        exitingSlope = true;

        //Zera a componente vertical da velocidade antes de aplicar a for�a de pulo
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        //Permite o pr�ximo pulo
        readyToJump = true;

        exitingSlope = false;
    }



    //RAMPA
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); //Define qu�o ingrime � a rampa
            return angle < maxSlopeAngle && angle != 0; //Retorna se � uma rampa ou n�o
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized; //Normalizar, j� que � uma dire��o
    }



    //ESTADOS DE MOVIMENTO
    private void StateHandler()
    {
        //MODO - Correndo
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //MODO - Andando
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        //MODO - Agachado
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        //MODO - No ar
        if (!isGrounded)
        {
            state = MovementState.inAir;
        }
    }

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        inAir
    }
}
