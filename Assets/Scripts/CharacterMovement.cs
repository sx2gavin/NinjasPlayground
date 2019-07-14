using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private const float BIG_EPSILON = 0.00001f;

    [SerializeField] CameraRig m_cameraRig;
    [SerializeField] float m_topSpeed = 5.0f;
    [SerializeField] float m_acceleration = 10.0f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;
    [SerializeField] float m_jumpHeight = 20.0f;
    [SerializeField] float m_jumpDelay = 0.3f;

    Animator m_animator;
    Rigidbody m_rigidBody;
    float m_currentSpeed = 0.0f;
    bool m_isInAir = false;
    bool m_isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Update()
    {
        Attack();
    }

    public void BeginAttacking()
    {
        m_isAttacking = true;
    }

    public void EndAttacking()
    {
        m_isAttacking = false;
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_animator.SetTrigger("attack");
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && !m_isInAir)
        {
            InstantJump();
        }
    }

    private IEnumerator DelayJumping()
    {
        m_animator.SetTrigger("jump");
        yield return new WaitForSeconds(m_jumpDelay);
        var currentVelocity = m_rigidBody.velocity;
        currentVelocity.y = m_jumpHeight;
        m_rigidBody.velocity = currentVelocity;
    }

    private void InstantJump()
    {
        m_animator.SetTrigger("jump");
        var currentVelocity = m_rigidBody.velocity;
        currentVelocity.y = m_jumpHeight;
        m_rigidBody.velocity = currentVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_isInAir = false;
            m_animator.SetBool("inAir", m_isInAir);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_isInAir = true;
            m_animator.SetBool("inAir", m_isInAir);
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (direction.magnitude >= BIG_EPSILON && !m_isAttacking)
        {
            direction.Normalize();
            var cameraRotation = m_cameraRig.transform.eulerAngles;

            direction = Quaternion.Euler(0, cameraRotation.y, 0) * direction;
            var rotateTo = Quaternion.FromToRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, playerRotationSmoothness);
            if (m_currentSpeed < m_topSpeed)
            {
                m_currentSpeed += m_acceleration * Time.deltaTime;
            }
            var vectorSpeed = direction * m_currentSpeed;
            vectorSpeed.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = vectorSpeed;

            m_animator.SetFloat("speedPercent", m_currentSpeed / m_topSpeed);
        }
        else
        {
            if (m_currentSpeed > 0.0f)
            {
                m_currentSpeed -= 2 * m_acceleration * Time.deltaTime;
                if (m_currentSpeed < 0.0f)
                {
                    m_currentSpeed = 0.0f;
                }
                var playerDirection = transform.forward * m_currentSpeed;
                m_rigidBody.velocity = new Vector3(playerDirection.x, m_rigidBody.velocity.y, playerDirection.z);
            }
            else
            {
                m_currentSpeed = 0.0f;
            }
            m_animator.SetFloat("speedPercent", m_currentSpeed / m_topSpeed);
        }
    }
}
