using System.Collections;
using System.Collections.Generic;
using PaintWars.FPS.Gameplay;
using UnityEngine;

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

    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    Vector3 m_GroundNormal;
    float m_LastTimeJumped = 0f;
    float m_CharacterHeight = 1.8f;
    float m_CameraVerticalAngle = 0f;

    const float k_GroundCheckDistanceInAir = 0.07f;
    const float k_JumpGroundingPreventionTime = 0.2f;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Controller.enableOverlapRecovery = true;

        m_InputHandler = GetComponent<PlayerInputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        bool wasGrounded = isGrounded;
        GroundCheck();

        UpdateCharacterHeight(false);

        HandleCharacterMovement();
    }

    void UpdateCharacterHeight(bool force)
    {
        if (force)
        {
            m_Controller.height = 1.8f;
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * m_CharacterHeight;

        }
        else if (m_Controller.height! != m_CharacterHeight)
        {
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_CharacterHeight, Time.deltaTime);
            m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
            Vector3.up * m_CharacterHeight * CameraHeightRatio, Time.deltaTime);
        }
    }

    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    Vector3 GetCapsuleUpperHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    void GroundCheck()
    {
        float chosenGroundCheckDistence = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        isGrounded = false;
        m_GroundNormal = Vector3.up;

        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            Vector3 p1 = transform.position + m_Controller.center + Vector3.up * -m_Controller.height * 0.5F;
            Vector3 p2 = p1 + Vector3.up * m_Controller.height;
            if (Physics.CapsuleCast(p1, p2, m_Controller.radius, Vector3.down,
                out RaycastHit hit, chosenGroundCheckDistence, groundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {
                m_GroundNormal = hit.normal;
                if (Vector3.Dot(m_GroundNormal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    m_Controller.Move(Vector3.down * hit.distance);
                }

            }
        }
    }

    void HandleCharacterMovement()
    {
        // Horizontal character rotation
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * RotationSpeed * rotationMultiplier), 0f), Space.Self);

        // Vertical camera rotation
        m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * rotationMultiplier;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);
        playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0f, 0f);

        bool isSprinting = m_InputHandler.GetSprintInputHeld();
        float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

        // Converts move input into worldspace vector
        Vector3 worldSpaceMove = transform.TransformVector(m_InputHandler.GetMoveInput());
        if (isGrounded)
        {
            Vector3 targetVelocity = worldSpaceMove * maxSpeedOnGround * speedModifier;
            targetVelocity = ReorientToSlopeDirection(targetVelocity, m_GroundNormal) * targetVelocity.magnitude;



        }
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