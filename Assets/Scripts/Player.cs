using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] float speed = 500.0f;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] float jumpHeight = 20.0f;
    Rigidbody m_rigidBody;
    bool m_isInAir = false;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_isInAir = CheckInAIr();
        Move();
        Jump();
    }

    private bool CheckInAIr()
    {
        return Math.Abs(m_rigidBody.velocity.y) >= 0.001;
    }

    private void Move()
    {
        if (m_isInAir == false)
        {
            float right = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            var movingVelocity = new Vector3(right, 0, forward);
            var cameraRotation = cameraRig.transform.eulerAngles;
            movingVelocity = Quaternion.Euler(0, cameraRotation.y, 0) * movingVelocity;
            movingVelocity.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = movingVelocity;
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
}
