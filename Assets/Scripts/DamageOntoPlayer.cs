using UnityEngine;

public class DamageOntoPlayer : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;
    public float knockbackForce = 0.8f;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player != null)
        {
            // You need to implement TakeDamage in ThirdPersonMovement if not present
            player.TakeDamage(damage, transform.position, knockbackForce);
        }
    }
}