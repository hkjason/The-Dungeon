using UnityEngine;
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _attackRate;

    public WeaponType weaponType;


    public LayerMask compareLayer;


    private void OnTriggerEnter(Collider other)
    {
        if ((1 << (other.gameObject.layer) & compareLayer) == (1 << (other.gameObject.layer)))
        {
            if (other.TryGetComponent(out Character character))
            {
                character.Damage(_damage);
            }
            else
            {
                Debug.Log("FAILURE");
            }
        }
    }

    public void SetColliderActive(bool tf)
    {
        GetComponent<BoxCollider>().enabled = tf;
    }

    public float GetAttackRate()
    {
        return _attackRate;
    }
}

public enum WeaponType
{
    OneHanded,
    TwoHanded
}
