using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeHandler : MonoBehaviour
{
    public Animator animator;
    public void StartFadeOut(Action CallOnEnd)
    {
        animator.SetTrigger("FadeOut");
        StartCoroutine(CallFadeOutEnd(CallOnEnd));
    }

    public void StartFadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
    IEnumerator CallFadeOutEnd(Action callOnEnd)
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        callOnEnd?.Invoke();

    }

}
