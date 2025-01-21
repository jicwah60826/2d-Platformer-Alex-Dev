using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Fruits Management")]
    public bool assignRandomFruit;
    public int fruitsCollected;

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
    public bool AssignRandomFruit() => assignRandomFruit;

    public void RespawnPlayer() => StartCoroutine(RespawnCo());

    private IEnumerator RespawnCo()
    {
        yield return new WaitForSeconds(respawnDelay);

        // instantiate the playter prefab as a var gameobject called newPlayer
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        // set the player var in the inspector to the object it finds the player method on
        player = newPlayer.GetComponent<Player>();
    }


}
