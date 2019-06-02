using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Enemy : MonoBehaviour
{
    [SerializeField] Player m_player;
    [SerializeField] float m_farDistanceThreshHold = 10f;
    [SerializeField] float m_closeDistanceThreshHold = 3f;
    [SerializeField] float m_attackRange = 3f;
    [SerializeField] float m_movingSpeed = 200f;
    [SerializeField] float m_interpolationRate = 0.5f;

    Node m_behaviorTree;
    float m_distanceWithPlayer;

    Rigidbody m_rigidbodyComponent;
    Animator m_animator;

    private void Awake()
    {
        DefineBehaviorTree();
    }

    private void DefineBehaviorTree()
    {
        // Behavior Tree definition
        m_behaviorTree = new Selector();
        var attackNode = new Action(Attack);
        var moveNode = new Action(Move);
        (m_behaviorTree as Selector).Children.Add(attackNode);
        (m_behaviorTree as Selector).Children.Add(moveNode);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbodyComponent = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player!= null)
        {
            m_distanceWithPlayer = Vector3.Distance(m_player.transform.position, transform.position);
        }

        m_behaviorTree.Perform();
    }

    private bool Move()
    {
        if (m_distanceWithPlayer < m_farDistanceThreshHold && m_distanceWithPlayer > m_closeDistanceThreshHold)
        {
            var fromMeToTarget = m_player.transform.position - transform.position;
            fromMeToTarget.y = 0;
            fromMeToTarget.Normalize();

            var newRotation = Quaternion.FromToRotation(Vector3.forward, fromMeToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
            m_rigidbodyComponent.velocity = (1 - m_interpolationRate) *
                m_rigidbodyComponent.velocity + m_interpolationRate * m_movingSpeed * fromMeToTarget * Time.deltaTime;
            return true;
        }
        else
        {
            m_rigidbodyComponent.velocity = Vector3.zero;
            return false;
        }
    }

    private bool Attack()
    {
        if (m_distanceWithPlayer < m_attackRange)
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
}
