using UnityEngine;

public class LevelEndpoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private bool isActive;

    [SerializeField]
    private GameObject blocker;

    [SerializeField]
    private GameObject levelEndVfx;

    [SerializeField]
    private GameObject levelEndFxPoint;

    private void Awake()
    {
        blocker.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive)
            return; //if already active - do nothing

        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            Debug.Log("Level Over");
            isActive = true;
            blocker.SetActive(true);
            anim.SetTrigger("activated");
            Instantiate(levelEndVfx, levelEndFxPoint.transform.position, Quaternion.identity);
        }
    }
}
