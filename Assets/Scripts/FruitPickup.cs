using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    [SerializeField] private int fruitAmount;

    private bool isCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isCollected)
        {

            isCollected = true;
            Destroy(gameObject);
        }
    }
}
