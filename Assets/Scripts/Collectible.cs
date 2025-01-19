using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField] private int collectibleAmount;

    private bool isCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !isCollected)
        {

            isCollected = true;
            GameManager.instance.CollectiblePickup(collectibleAmount);
            Destroy(gameObject);
        }
    }
}
