using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int m_damage = 10;

    public bool IsSwinging { get; set; } = false;

    public int GetDamage()
    {
        return IsSwinging ? m_damage : 0;
    }

    // Swing will activate the weapon to apply damage on others.
    public void Swing()
    {
        IsSwinging = true;
    }

    // EndSwing will deactivate the weapon to stop it from damaging others.
    public void EndSwing()
    {
        IsSwinging = false;
    }
}