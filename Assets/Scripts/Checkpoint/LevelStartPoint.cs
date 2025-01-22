using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelStartPoint : MonoBehaviour
{

    private Animator anim => GetComponent<Animator>();

    private void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if(player != null)
        {
            anim.SetTrigger("activated");
        }
    }
}
