using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] GameObject m_hand;
    [SerializeField] GameObject m_weaponPrefab;

    GameObject m_weaponInstance;
    Weapon m_weapon;

    private void Start()
    {
        m_weaponInstance = Instantiate(m_weaponPrefab, m_hand.transform.position, m_hand.transform.rotation);
        m_weapon = m_weaponInstance.GetComponent<Weapon>();
    }

    private void Update()
    {
        m_weaponInstance.transform.position = m_hand.transform.position;
        m_weaponInstance.transform.rotation = m_hand.transform.rotation;
    }

    private void BeginSwing()
    {
        if (m_weapon)
        {
            m_weapon?.Swing();
        }
    }

    private void EndSwing()
    {
        if (m_weapon)
        {
            m_weapon?.EndSwing();
        }
    }
}
