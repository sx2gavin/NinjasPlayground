using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] int m_hitPoints = 100;
    [SerializeField] Slider m_healthSlider;

    int m_currentHitPoints;

    private void Start()
    {
        m_currentHitPoints = m_hitPoints;
        if (m_healthSlider == null)
        {
            m_healthSlider = GetComponentInChildren<Slider>();
        }
    }

    public void TakeDamage(int damage)
    {
        m_currentHitPoints -= damage;
        if (m_healthSlider != null)
        {
            m_healthSlider.value = m_currentHitPoints / (float)m_hitPoints;
        }
    }

    public int GetCurrentHitPoints()
    {
        return m_currentHitPoints;
    }
}
