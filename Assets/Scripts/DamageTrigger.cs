using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("DamageTrigger entered");

        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Debug.Log("Player Damaged!");
            player.KnockBack(transform.position.x);
        }
    }
}
