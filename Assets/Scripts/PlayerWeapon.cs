using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] GameObject m_hand;
    [SerializeField] GameObject m_weaponPrefab;

    GameObject m_weaponInstance;

    private void Start()
    {
        m_weaponInstance = Instantiate(m_weaponPrefab, m_hand.transform.position, m_hand.transform.rotation);
    }

    private void Update()
    {
        m_weaponInstance.transform.position = m_hand.transform.position;
        m_weaponInstance.transform.rotation = m_hand.transform.rotation;
    }
}
