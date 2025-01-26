using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform respawnPoint;

    [SerializeField]
    private float respawnDelay;
    public Player player;

    [Header("Fruits Management")]
    public bool assignRandomFruit;
    public int fruitsCollected;
    public int totalFruits;

    [Header("Checkpoints")]
    public bool canReactivate;

    [Header("Traps")]
    public GameObject arrowPrefab;

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

    private void Start()
    {
        GetAllFruits();
    }

    private void GetAllFruits()
    {
        // find all fruits in scene
        FruitPickup[] ar_allFruits = FindObjectsByType<FruitPickup>(FindObjectsSortMode.None);
        totalFruits = ar_allFruits.Length;
    }

    public void AddFruit() => fruitsCollected++;

    public bool AssignRandomFruit() => assignRandomFruit;

    public void UpdateRespawnPosition(Transform newrespawnPoint) => respawnPoint = newrespawnPoint;

    public void RespawnPlayer() => StartCoroutine(RespawnCo());

    private IEnumerator RespawnCo()
    {
        yield return new WaitForSeconds(respawnDelay);

        // instantiate the playter prefab as a var gameobject called newPlayer
        GameObject newPlayer = Instantiate(
            playerPrefab,
            respawnPoint.position,
            Quaternion.identity
        );

        // set the player var in the inspector to the object it finds the player method on
        player = newPlayer.GetComponent<Player>();
    }

    public void CreateObject(GameObject prefab, Transform target, float delay = 0f)
    {
        StartCoroutine(CreateObjectCo(prefab, target, delay));
    }

    private IEnumerator CreateObjectCo(GameObject prefab, Transform target, float delay = 0f)
    {
        Vector3 newPosition = target.position; // save location BEFORE we do the delay

        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
    }
}
