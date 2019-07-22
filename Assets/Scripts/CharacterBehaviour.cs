using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehaviour : MonoBehaviour
{
    Energy m_energy;
    Health m_health;

    private void Start()
    {
        m_health = GetComponent<Health>();
        m_energy = GetComponent<Energy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.GetComponent<EnemyWeapon>();
        if (weapon != null)
        {
            m_health.TakeDamage(weapon.GetDamage());
            if (m_health.GetCurrentHitPoints() <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
