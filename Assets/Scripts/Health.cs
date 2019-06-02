using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] int m_hitPoints = 100;
    [SerializeField] Slider m_healthSlider;

    [SerializeField] int m_currentHitPoints;

    Weapon m_weapon = null;

    private void Start()
    {
        m_currentHitPoints = m_hitPoints;
        if (m_healthSlider == null)
        {
            m_healthSlider = GetComponentInChildren<Slider>();
        }
    }

    private void TakeDamage(int damage)
    {
        m_currentHitPoints -= damage;
        if (m_healthSlider != null)
        {
            m_healthSlider.value = m_currentHitPoints / (float)m_hitPoints;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var weapon = other.GetComponent<Weapon>();
        if (weapon != null && weapon != m_weapon)
        {
            m_weapon = weapon;
            TakeDamage(weapon.GetDamage());
            if (m_currentHitPoints <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var weapon = other.GetComponent<Weapon>();
        if (weapon == m_weapon)
        {
            m_weapon = null;
        }
    }
}
