using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int fruitCount;

    public Player player;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        player = FindAnyObjectByType<Player>();
    }


    public void CollectiblePickup(int amount)
    {
        fruitCount += amount;
    }


}
