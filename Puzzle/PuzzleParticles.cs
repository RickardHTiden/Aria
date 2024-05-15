using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PuzzleParticles : MonoBehaviour
{

    [SerializeField] private List<VisualEffect> effects = new List<VisualEffect>();

    public void Play()
    {
        foreach(VisualEffect vfx in effects)
        {
            vfx.Play();
        }
    }
}
