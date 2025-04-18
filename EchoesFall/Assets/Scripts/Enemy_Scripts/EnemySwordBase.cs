using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordBase : MonoBehaviour
{
     public float damage;
    public Collider bladeCollider;

    // Start is called before the first frame update
    void Start()
    {
        bladeCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnableCollider()
    {
        bladeCollider.enabled = true;
    }
    public void DisableCollider()
    {
        bladeCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        
        PlayerController player = other.GetComponent<PlayerController>();
        if(player != null)
        {
            player.TakeDamage(damage);
        }
    }
}
