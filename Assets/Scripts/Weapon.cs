using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int m_damage = 10;

    public bool IsSwinging { get; set; } = false;

    public int GetDamage()
    {
        return IsSwinging ? m_damage : 0;
    }

    public void Swing()
    {
        IsSwinging = true;
    }

    public void EndSwing()
    {
        IsSwinging = false;
    }
}