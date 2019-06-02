using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private const float BIG_EPSILON = 0.00001f;

    [SerializeField] CameraRig cameraRig;
    [SerializeField] Throwable throwableWeapon;
    [SerializeField] float speed = 500.0f;
    [SerializeField] float acceleration = 2.0f;
    [SerializeField] float jumpHeight = 20.0f;
    [SerializeField] float dodgeDistance = 20.0f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;

    Rigidbody m_rigidBody;
    Animator m_animator;
    Weapon m_weapon;
    bool m_isInAir = false;
    bool m_isDodging = false;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_weapon = GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Blocking();
        Throw();
    }

    private void Blocking()
    {
        if (Input.GetButtonDown("Block"))
        {
            m_animator.SetBool("IsBlocking", true);
            m_animator?.Play("Start Blocking");
        }
        else if (Input.GetButtonUp("Block"))
        {
            m_animator?.SetBool("IsBlocking", false);
        }
    }

    private void Dodge()
    {
        if (Input.GetButtonDown("Dodge"))
        {
            StartCoroutine(DodgeOvertime());
        }
    }

    private IEnumerator DodgeOvertime()
    {
        m_isDodging = true;
        m_rigidBody.velocity = Vector3.zero;

        float travelledDistance = 0.0f;

        while (travelledDistance < dodgeDistance)
        {
            var dodgedDistanceThisFrame = dodgeSpeed * Time.deltaTime;
            transform.position += transform.forward * dodgedDistanceThisFrame;
            travelledDistance += dodgedDistanceThisFrame;
            yield return new WaitForFixedUpdate();
        }

        m_isDodging = false;
    }

    private void Throw()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var inFrontOfPlayer = transform.position + transform.forward * 2 + transform.up;
            var throwable = Instantiate(throwableWeapon, inFrontOfPlayer, transform.rotation);
            throwable.Fly(inFrontOfPlayer, transform.forward);
        }
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_animator?.SetTrigger("Attack");
        }
    }

    private void BeginAttacking()
    {
        m_weapon?.Swing();
    }

    private void EndAttacking()
    {
        m_weapon?.EndSwing();
    }

    private void FixedUpdate()
    {
        m_isInAir = CheckInAIr();
        Dodge();
        if (!m_isDodging)
        {
            Move();
        }
        Jump();
    }

    private bool CheckInAIr()
    {
        return Math.Abs(m_rigidBody.velocity.y) >= BIG_EPSILON;
    }

    private void Move()
    {
        float right = Mathf.Clamp(Input.GetAxis("Horizontal") * acceleration, -1f, 1f) * speed * Time.deltaTime;
        float forward = Mathf.Clamp(Input.GetAxis("Vertical") * acceleration, -1f, 1f) * speed * Time.deltaTime;

        if (Math.Abs(right) >= BIG_EPSILON || Math.Abs(forward) >= BIG_EPSILON)
        {
            var movingVelocity = new Vector3(right, 0, forward);

            var cameraRotation = cameraRig.transform.eulerAngles;
            movingVelocity = Quaternion.Euler(0, cameraRotation.y, 0) * movingVelocity;

            var rotateTo = Quaternion.FromToRotation(Vector3.forward, movingVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, playerRotationSmoothness);


            movingVelocity.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = movingVelocity;

            m_animator.SetBool("IsRunning", true);
        }
        else
        {
            m_rigidBody.velocity = new Vector3(0, m_rigidBody.velocity.y, 0);
            m_animator.SetBool("IsRunning", false);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && !m_isInAir)
        {
            var currentVelocity = m_rigidBody.velocity;
            currentVelocity.y = jumpHeight;
            m_rigidBody.velocity = currentVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_isInAir = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            m_isInAir = true;
        }
    }
}
