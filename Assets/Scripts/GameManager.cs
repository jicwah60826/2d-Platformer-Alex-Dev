using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int fruitsCollected;


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


        player = FindAnyObjectByType<Player>(); //auto assign Player
    }

    public void AddFruit() => fruitsCollected++;


}
