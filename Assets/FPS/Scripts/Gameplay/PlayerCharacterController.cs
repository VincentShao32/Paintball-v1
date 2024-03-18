using System;
using System.Collections;
using System.Collections.Generic;
using PaintWars.FPS.Game;
using PaintWars.FPS.Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;
    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.05f;
    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;
    [Tooltip("Max movement speed when not grounded")]
    public float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;

    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;
    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float RotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float AimingRotationMultiplier = 0.4f;
    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float JumpForce = 9f;

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float CameraHeightRatio = 0.9f;

    [Tooltip("Height of character when standing")]
    public float CapsuleHeightStanding = 1.8f;

    [Header("Fall Damage")]
    [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
    public bool RecievesFallDamage;

    [Tooltip("Minimun fall speed for recieving fall damage")]
    public float MinSpeedForFallDamage = 10f;

    [Tooltip("Fall speed for recieving the maximum amount of fall damage")]
    public float MaxSpeedForFallDamage = 30f;

    [Tooltip("Damage recieved when falling at the mimimum speed")]
    public float FallDamageAtMinSpeed = 10f;

    [Tooltip("Damage recieved when falling at the maximum speed")]


    public float FallDamageAtMaxSpeed = 50f;

    public Vector3 characterVelocity { get; set; }

    public bool isGrounded { get; private set; }
    public bool isDead { get; private set; }
    public float rotationMultiplier
    {
        get
        {
            // if aiming return AimingRotationMultiplier
            return 1f;
        }
    }

    //PlayerInputHandler m_InputHandler;
    Health m_Health;
    CharacterController m_Controller;
    PlayerInputContainer m_InputContainer;
    Vector3 m_GroundNormal;
    float m_LastTimeJumped = 0f;
    float m_CameraVerticalAngle = 0f;

    const float k_GroundCheckDistanceInAir = 0.08f;
    const float k_JumpGroundingPreventionTime = 0.2f;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Controller.enableOverlapRecovery = true;

        //m_InputHandler = GetComponent<PlayerInputHandler>();
        m_InputContainer = GetComponent<PlayerInputContainer>();
        characterVelocity = new Vector3(0, 0, 0);
        m_Health = GetComponent<Health>();
        m_Health.OnDie += OnDie;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        HandleCharacterMovement();

    }

    void OnDie()
    {
        isDead = true;
        this.enabled = false;
    }

    void GroundCheck()
    {
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        isGrounded = false;
        m_GroundNormal = Vector3.up;

        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            Vector3 p1 = transform.position + m_Controller.center + Vector3.up * -m_Controller.height * 0.5F + new Vector3(0, m_Controller.radius, 0);
            Vector3 p2 = p1 + Vector3.up * m_Controller.height - new Vector3(0, m_Controller.radius, 0);
            Debug.DrawLine(p1, p1 + Vector3.down * chosenGroundCheckDistance, Color.green, 1);

            if (Physics.CapsuleCast(p1, p2, m_Controller.radius, Vector3.down,
                out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {
                m_GroundNormal = hit.normal;
                if (Vector3.Dot(m_GroundNormal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);

                    }
                }
            }
        }
    }

    void HandleCharacterMovement()
    {
        // Horizontal character rotation
        transform.Rotate(new Vector3(0f, m_InputContainer.lookVector.x * RotationSpeed * rotationMultiplier, 0f), Space.Self);

        // Vertical camera rotation
        m_CameraVerticalAngle += m_InputContainer.lookVector.y * RotationSpeed * rotationMultiplier;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);
        playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);

        // Converts move input into worldspace vector
        characterVelocity = transform.TransformVector(new Vector3(
            m_InputContainer.characterMovementTransform.x,
            characterVelocity.y,
            m_InputContainer.characterMovementTransform.z
        ));
        if (isGrounded)
        {
            characterVelocity -= Vector3.up * characterVelocity.y;
            characterVelocity *= maxSpeedOnGround;
            characterVelocity = ReorientToSlopeDirection(characterVelocity, m_GroundNormal) * characterVelocity.magnitude;

            if (m_InputContainer.jumped)
            {
                m_LastTimeJumped = Time.time;
                characterVelocity += Vector3.up * JumpForce;
                m_InputContainer.resetJump();
            }

        }

        else
        {
            characterVelocity = new Vector3(characterVelocity.x * maxSpeedInAir, characterVelocity.y, characterVelocity.z * maxSpeedInAir);
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }
        m_Controller.Move(characterVelocity * Time.deltaTime);
    }

    public Vector3 ReorientToSlopeDirection(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 rightDirection = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, rightDirection).normalized;
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }
}
