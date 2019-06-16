using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Action = BehaviorTree.Action;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] float m_closeDistanceThreshHold = 1f;
    [SerializeField] float m_attackRange = 2f;
    [SerializeField] float m_movingSpeed = 400f;
    [SerializeField]
    [Tooltip("Interpolate movement.")]
    float m_movementInterpolation = 0.5f;
    [SerializeField]
    [Tooltip("If player is within the range, enemy will start moving towards it.")]
    float m_playerDetectionRadius = 20f;
    [SerializeField]
    [Tooltip("Vision angle for enemy to detect player.")]
    float m_detectionAngle = 90f;

    Player m_player;
    Node m_behaviorTree;
    float m_distanceWithPlayer;
    Rigidbody m_rigidbodyComponent;
    Animator m_animator;
    Weapon m_weapon;
    Health m_health;
    bool m_isAttacking = false;

    private void Awake()
    {
        DefineBehaviorTree();
    }

    private void DefineBehaviorTree()
    {
        // Behavior Tree definition
        m_behaviorTree = new Sequencor();
        var findPlayer = new Action(FindPlayer);
        m_behaviorTree.AddChild(findPlayer);
        var actionNode = new Selector();
        var attackNode = new Action(Attack);
        var moveNode = new Action(Move);
        actionNode.AddChild(attackNode);
        actionNode.AddChild(moveNode);

        m_behaviorTree.AddChild(actionNode);
    }

    private bool FindPlayer()
    {
        if (m_player == null)
        {
            var player = FindPlayerWithinRange();
            if (player != null && CheckIfPlayerWithinVision(player))
            {
                m_player = player;
            }
        }

        if (m_player != null)
        {
            
            m_distanceWithPlayer = Vector3.Distance(m_player.transform.position, transform.position);
            return true;
        }
        return false;
    }

    private Player FindPlayerWithinRange()
    {
        var colliders = Physics.OverlapSphere(transform.position, m_playerDetectionRadius);

        foreach (var collider in colliders)
        {
            var player = collider.GetComponent<Player>();
            if (player != null)
            {
                return player;
            }
        }

        return default;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbodyComponent = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_weapon = GetComponentInChildren<Weapon>();
        m_health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        m_behaviorTree.Perform();
    }

    private bool CheckIfPlayerWithinVision(Player player)
    {
        if (player != null)
        {
            var playerDirection = player.transform.position - transform.position;
            var angle = Vector3.Angle(playerDirection, transform.forward);
            if (angle < m_detectionAngle)
            {
                return true;
            }
        }

        return false;
    }

    private bool Move()
    {
        if (m_player != null)
        {
            if (m_distanceWithPlayer < m_playerDetectionRadius && m_distanceWithPlayer > m_closeDistanceThreshHold && !m_isAttacking)
            {
                var fromMeToTarget = m_player.transform.position - transform.position;
                fromMeToTarget.y = 0;
                fromMeToTarget.Normalize();

                var newRotation = Quaternion.FromToRotation(Vector3.forward, fromMeToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
                var newVelocity = m_movementInterpolation * m_movingSpeed * fromMeToTarget * Time.deltaTime;
                m_rigidbodyComponent.velocity = Vector3.Lerp(m_rigidbodyComponent.velocity, newVelocity, m_movementInterpolation);
                return true;
            }
            else
            {
                m_rigidbodyComponent.velocity = Vector3.zero;
                return false;
            }
        }

        return false;
    }

    private bool Attack()
    {
        if (m_player != null && m_distanceWithPlayer < m_attackRange)
        {
            m_rigidbodyComponent.velocity = Vector3.zero;
            m_animator?.Play("Attack");
            return true;
        }
        else
        {
            return false;
        }
    }

    private void BeginAttacking()
    {
        m_isAttacking = true;
        m_weapon?.Swing();
    }

    private void EndDamage()
    {
        m_weapon?.EndSwing();
    }

    private void EndAttacking()
    {
        m_isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.GetComponent<PlayerWeapon>();
        if (weapon != null)
        {
            m_health.TakeDamage(weapon.GetDamage());
            StartCoroutine(PauseGame(0.05f));
            var swordParticles = weapon.GetSwordParticles();
            Instantiate(swordParticles, weapon.transform.position, weapon.transform.rotation);
            if (m_health.GetCurrentHitPoints() <= 0)
            {
                Destroy(gameObject, 1.0f);
            }
        }
    }

    private IEnumerator PauseGame(float seconds)
    {
        Time.timeScale = 0.0f;
        float pauseEndTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1.0f;
    }
}
