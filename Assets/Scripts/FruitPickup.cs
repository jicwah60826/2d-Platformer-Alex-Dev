using Unity.VisualScripting;
using UnityEngine;

public enum fruitType { Apple, Banana, Cherry, Kiwi, Melon, Orange, Pineapple, Strawberry }

public class FruitPickup : MonoBehaviour
{
    [SerializeField] private fruitType fruitType;
    [SerializeField] private GameObject pickupVfx;

    private GameManager gameManager;
    private Animator anim;
    private bool isCollected;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        SetRandomFruit();
    }

    public void SetRandomFruit()
    {

        if (!gameManager.AssignRandomFruit())
        {
            UpdateFruitVisuals();
            return;
        }
            int randomIndex = Random.Range(0, 8);
            anim.SetFloat("fruitIndex", randomIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isCollected)
        {

            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                isCollected = true;
                gameManager.AddFruit();
                Destroy(gameObject);
                Instantiate(pickupVfx,transform.position,Quaternion.identity);
            }

        }
    }

    private void UpdateFruitVisuals() => anim.SetFloat("fruitIndex", (int)fruitType);

}
