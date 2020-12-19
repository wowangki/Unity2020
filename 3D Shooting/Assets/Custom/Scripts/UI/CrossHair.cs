using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void WalkingAnim(bool flag)
    {
        anim.SetBool("IsWalk", flag);
    }

    public void RunningAnim(bool flag)
    {
        anim.SetBool("IsRun", flag);
    }

    public void FireAnim()
    {
        anim.SetTrigger("Fire");
    }

    public void ChangeGunType(GunProperty.GUN_KIND kind)
    {
        anim.SetInteger("GunType", (int)kind);
    }
}
