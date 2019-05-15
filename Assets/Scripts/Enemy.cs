using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] Player player;
    [SerializeField] float farDistanceThreshHold = 10f;
    [SerializeField] float closeDistanceThreshHold = 1f;
    [SerializeField] float movingSpeed = 200f;
    [SerializeField] float interpolationRate = 0.5f;

    Rigidbody rigidbodyComponent;
    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player!= null)
        {
            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < farDistanceThreshHold && distance > closeDistanceThreshHold)
            {
                var fromMeToTarget = player.transform.position - transform.position;
                fromMeToTarget.Normalize();
                rigidbodyComponent.velocity = (1-interpolationRate) * rigidbodyComponent.velocity + interpolationRate * movingSpeed * fromMeToTarget * Time.deltaTime;
            }
        }
    }
}
