using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private const float BIG_EPSILON = 0.00001f;

    [SerializeField] CameraRig m_cameraRig;
    [SerializeField] Throwable throwableWeapon;
    [SerializeField] float speed = 500.0f;
    [SerializeField] float acceleration = 2.0f;
    [SerializeField] float jumpHeight = 20.0f;
    [SerializeField] float dodgeDistance = 20.0f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;
    [SerializeField] float m_enemyLockingRange = 20.0f;

    [SerializeField] int m_dodgeEnergyConsumption = 10;
    [SerializeField] int m_attackEnergyConsumption = 20;
    [SerializeField] int m_blockingEnergyConsumption = 3;

    Rigidbody m_rigidBody;
    Animator m_animator;
    Weapon m_weapon;
    bool m_isInAir = false;
    bool m_isDodging = false;
    bool m_isAttacking = false;
    bool m_isLockingTarget = false;
    Energy m_energy;
    Health m_health;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_weapon = GetComponentInChildren<Weapon>();
        m_health = GetComponent<Health>();
        m_energy = GetComponent<Energy>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Blocking();
        Target();
        Throw();
    }

    private void Target()
    {
        if (Input.GetButtonDown("Lock Target"))
        {
            m_isLockingTarget = !m_isLockingTarget;
        }

        if (m_isLockingTarget)
        {
            var targetableObjects = FindNearbyTargetableObjects();
            if (targetableObjects.Count == 0)
            {
                m_isLockingTarget = false;
            }
            m_cameraRig.Targeting(targetableObjects);
        }
        else
        {
            m_cameraRig.Targeting(null);
        }
    }

    private List<TargetableObject> FindNearbyTargetableObjects()
    {
        var colliders = Physics.OverlapSphere(transform.position, m_enemyLockingRange);

        var targetableObjects = new List<TargetableObject>();
        foreach (var collider in colliders)
        {
            var targetableObject = collider.GetComponent<TargetableObject>();
            if (targetableObject != null)
            {
                targetableObjects.Add(targetableObject);
            }
        }

        return targetableObjects;
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

        if (m_animator.GetBool("IsBlocking"))
        {
            if (!m_energy.ConsumeEnergyPerSecond(m_blockingEnergyConsumption, Time.deltaTime))
            {
                m_animator?.SetBool("IsBlocking", false);
            }
        }
    }

    private void Dodge()
    {
        if (Input.GetButtonDown("Dodge"))
        {
            if (m_energy.ConsumeEnergy(m_dodgeEnergyConsumption))
            {
                StartCoroutine(DodgeOvertime());
            }
        }
    }

    private IEnumerator DodgeOvertime()
    {
        m_isDodging = true;
        m_rigidBody.velocity = new Vector3(0, m_rigidBody.velocity.y, 0);

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && !m_isAttacking)
        {
            if (m_energy.ConsumeEnergy(m_attackEnergyConsumption))
            {
                m_animator?.SetTrigger("Attack");
            }
        }
    }

    private void BeginSwing()
    {
        m_isAttacking = true;
        m_weapon?.Swing();
    }

    private void EndSwing()
    {
        m_weapon?.EndSwing();
    }

    private void EndAttackAnimation()
    {
        m_isAttacking = false;
    }

    private void FixedUpdate()
    {
        Dodge();
        if (!m_isDodging)
        {
            Move();
        }
        Jump();
    }

    private void Move()
    {
        float right = Mathf.Clamp(Input.GetAxis("Horizontal") * acceleration, -1f, 1f) * speed * Time.deltaTime;
        float forward = Mathf.Clamp(Input.GetAxis("Vertical") * acceleration, -1f, 1f) * speed * Time.deltaTime;

        if (Math.Abs(right) >= BIG_EPSILON || Math.Abs(forward) >= BIG_EPSILON)
        {
            var movingVelocity = new Vector3(right, 0, forward);

            var cameraRotation = m_cameraRig.transform.eulerAngles;
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

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.GetComponent<EnemyWeapon>();
        if (weapon != null)
        {
            var isBlocking = m_animator.GetBool("IsBlocking");
            if (!isBlocking)
            {
                m_health.TakeDamage(weapon.GetDamage());
                if (m_health.GetCurrentHitPoints() <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
