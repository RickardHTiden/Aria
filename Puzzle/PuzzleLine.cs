using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PuzzleLine : MonoBehaviour
{
    [SerializeField] private VisualEffect lineParticle;
    [SerializeField] private Animator anim;

    public void SetPosition(Vector3 pos, Quaternion puzzleRot)
    {
        pos = puzzleRot * pos;
        lineParticle.SetVector3("EndPos", pos);
    }
    public void SetPosition(Vector3 pos)
    {
        lineParticle.SetVector3("EndPos", pos);
    }

    public void TurnOffLine()
    {
        //animate something that calls on Stop
        anim.SetTrigger("off");
    }

    public void LineErasable(bool isErasable)
    {
        anim.SetBool("Erasable", isErasable);
    }

    public void Stop()
    {
        lineParticle.Stop();
    }
    public void DeleteLine()
    {
        Destroy(gameObject);
    }

    public void Play()
    {
        lineParticle.Play();
    }
}
