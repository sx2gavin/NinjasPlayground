using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    [SerializeField] int m_energyPoints = 100;
    [SerializeField] Slider m_energySlider;
    [SerializeField] int m_regenerateSpeed = 5;

    float m_currentEnergyPoints;
    float timeElapsed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_currentEnergyPoints = m_energyPoints;
        if (m_energySlider == null)
        {
            m_energySlider = GetComponentInChildren<Slider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentEnergyPoints < m_energyPoints)
        {
            m_currentEnergyPoints += m_regenerateSpeed * Time.deltaTime;
            UpdateSlider();
        }
    }

    void UpdateSlider()
    {
        if (m_energySlider != null)
        {
            m_energySlider.value = m_currentEnergyPoints / (float)m_energyPoints;
        }
    }

    public bool ConsumeEnergy(float energy)
    {
        if (energy <= m_currentEnergyPoints)
        {
            m_currentEnergyPoints -= energy;
            UpdateSlider();
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool ConsumeEnergyPerSecond(int energyPerSecond, float deltaTime)
    {
        float energy = energyPerSecond * deltaTime;
        return ConsumeEnergy(energy);
    }
}
