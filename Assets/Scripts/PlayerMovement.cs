using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;

    public InputAction MoveAction;
    public InputAction SprintAction;
    public InputAction JumpAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float sprintMultiplier = 1.8f;
    public float jumpImpulse = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask = ~0;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    bool m_IsGrounded;
    bool m_QueuedJump;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        if (SprintAction != null) SprintAction.Enable();
        if (JumpAction != null) JumpAction.Enable();
    }

    void Update()
    {
        if (JumpAction != null && JumpAction.triggered)
        {
            m_QueuedJump = true;
        }
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        // Ground check
        m_IsGrounded = Physics.Raycast(m_Rigidbody.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundMask, QueryTriggerInteraction.Ignore);

        // Jump
        if (m_QueuedJump && m_IsGrounded)
        {
            m_QueuedJump = false;
            m_Rigidbody.AddForce(Vector3.up * jumpImpulse, ForceMode.VelocityChange);
        }
        else
        {
            // Clear queued flag if we couldn't jump this frame (prevents sticky buffering when airborne for too long)
            m_QueuedJump = false;
        }

        // Sprinting affects planar speed
        float speed = walkSpeed;
        if (SprintAction != null && SprintAction.IsPressed())
        {
            speed *= sprintMultiplier;
        }

        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * speed * Time.deltaTime);
    }
}