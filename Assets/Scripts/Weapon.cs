using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int m_damage = 10;

    Collider m_collider;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;
    }

    public bool IsSwinging { get; set; } = false;

    public int GetDamage()
    {
        return m_damage;
    }

    // Swing will activate the weapon to apply damage on others.
    public void Swing()
    {
        m_collider.enabled = true;
    }

    // EndSwing will deactivate the weapon to stop it from damaging others.
    public void EndSwing()
    {
        m_collider.enabled = false;
    }
}