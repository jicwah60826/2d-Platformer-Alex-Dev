using Unity.VisualScripting;
using UnityEngine;

public class FruitPickup : MonoBehaviour
{

    private GameManager gameManager;
    private Animator animator;

    private bool isCollected;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isCollected)
        {

            isCollected = true;
            gameManager.AddFruit();
            Destroy(gameObject);
        }
    }
}
