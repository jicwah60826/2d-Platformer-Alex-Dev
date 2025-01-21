using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    private bool isActive;


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (isActive) return; //if already active - do nothing

        Player player = other.GetComponent<Player>();

        if(player != null)
        {
            ActivateCheckPoint();
        } 
    }

    private void ActivateCheckPoint()
    {
        anim.SetBool("active", isActive);
    }


}
