using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Character Stat")]
    [SerializeField]
    protected CharacterStat characterStat;

    [Header("Weapon")]
    public Weapon currentWeapon;

    [SerializeField]
    protected bool _isDead = false;

    public Animator animator;

    protected void Awake()
    {
        if (!TryGetComponent(out animator))
            Debug.Log("FAILURE");
    }

    private int HP
    {
        get { return characterStat.hp; }
        set 
        { 
            characterStat.hp = value;
            CheckDeath();
        }
    }

    public void Damage(int damage)
    {
        HP -= damage;
    }

    private void CheckDeath()
    {
        if (HP <= 0)
        {
            _isDead = true;
        }
        if (_isDead)
        {
            DeadTrigger();
        }
    }

    protected abstract void DeadTrigger();

    public float GetMovementSpeed()
    {
        return characterStat.movementSpeed;
    }
}
