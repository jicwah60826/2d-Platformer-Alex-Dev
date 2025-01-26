using UnityEngine;

public class Trap_Trampoline : MonoBehaviour
{
    [Header("Trampoline Mechanics")]
    [SerializeField]
    protected float pushPower;

    [SerializeField]
    protected float duration = .5f;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            anim.SetTrigger("Activate");
            player.Push(transform.up * pushPower, duration);
        }
    }
}
