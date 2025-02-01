using Unity.VisualScripting;
using UnityEngine;

public class Trap_Fire_Button : MonoBehaviour
{
    private Trap_Fire trapFire;
    private Animator anim;

    private void Awake()
    {
        trapFire = GetComponentInParent<Trap_Fire>();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {
            anim.SetTrigger("activate");
            trapFire.SwitchOffFire();
        }
    }
}
