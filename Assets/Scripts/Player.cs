using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField] CameraRig cameraRig;
    [SerializeField] Throwable throwableWeapon;
    [SerializeField] float speed = 500.0f;
    [SerializeField] float acceleration = 2.0f;
    [SerializeField] float jumpHeight = 20.0f;
    [SerializeField] float dodgeDistance = 20.0f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;

    Rigidbody m_rigidBody;
    Animator animator;
    bool m_isInAir = false;
    bool isDodging = false;
    private const float BIG_EPSILON = 0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Blocking();
        Dodge();
        Throw();
    }

    private void Blocking()
    {
        if (Input.GetButtonDown("Block"))
        {
            animator.SetBool("IsBlocking", true);
            animator?.Play("Start Blocking");
        } 
        else if (Input.GetButtonUp("Block"))
        {
            animator?.SetBool("IsBlocking", false);
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
        isDodging = true;
        m_rigidBody.velocity = Vector3.zero;

        float travelledDistance = 0.0f;

        while(travelledDistance < dodgeDistance)
        {
            var dodgedDistanceThisFrame = dodgeSpeed * Time.deltaTime;
            transform.position += transform.forward * dodgedDistanceThisFrame;
            travelledDistance += dodgedDistanceThisFrame;
            yield return null;
        }

        isDodging = false;
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
            animator?.Play("Attack");
        }
    }

    private void FixedUpdate()
    {
        m_isInAir = CheckInAIr();
        Move();
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

            animator.SetBool("IsRunning", true);
        }
        else
        {
            m_rigidBody.velocity = new Vector3(0, m_rigidBody.velocity.y, 0);
            animator.SetBool("IsRunning", false);
        }

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            var currentVelocity = m_rigidBody.velocity;
            currentVelocity.y = jumpHeight;
            m_rigidBody.velocity = currentVelocity;
        }
    }
}
