using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerBase : MonoBehaviour
{
    public float damage;
    public Collider daggerCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnableCollider()
    {
        daggerCollider.enabled = true;
    }
    public void DisableCollider()
    {
        daggerCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.CompareTag("Enemy"))
        {
            damage = other.GetComponent<EnemyController>().HP;

            iDamageable damageable = other.GetComponent<iDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
