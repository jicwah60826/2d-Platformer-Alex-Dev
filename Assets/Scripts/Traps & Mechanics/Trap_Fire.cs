using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Trap_Fire : MonoBehaviour
{
    [SerializeField]
    private float offDuration;

    // get a reference to the Trap Fire Button Class
    [SerializeField]
    private Trap_Fire_Button trapFireButton;

    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        if (trapFireButton == null)
        {
            Debug.LogError(
                "You have not set the Trap Fire Button reference on game object "
                    + gameObject.name
                    + "!"
            );
        }

        // start fire by default
        SetFire(true);
    }

    public void SwitchOffFire()
    {
        // if fire is on - switch it off, else don't do anything
        if (!isActive)
            return;

        StartCoroutine(FireCo());
    }

    private IEnumerator FireCo()
    {
        //turns fire off
        SetFire(false);

        //waits for cooldown amount
        yield return new WaitForSeconds(offDuration);

        // tuirns fire back on
        SetFire(true);
    }

    private void SetFire(bool active)
    {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
