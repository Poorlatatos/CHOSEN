using UnityEngine;

public class DamageOntoPlayer : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;
    public float knockbackForce = 0.8f;

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonMovement player = other.GetComponent<FirstPersonMovement>();
        if (player != null)
        {
            player.TakeDamage(damage, transform.position, knockbackForce);
        }
    }
}