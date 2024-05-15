using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HazardObject : MonoBehaviour
{
   //private Animator animator;
    
    [SerializeField] private bool movingBackwards;
    [SerializeField] private float moveTime = 1.2f;

    private Vector3 direction;
    private VisualEffect lavaParticles;

    public Vector3 StartPos { get; set; }
    public int PuzzleID { get; internal set; }

    private void Awake()
    {
        //animator = GetComponent<Animator>();   
        lavaParticles = GetComponent<VisualEffect>();
    }

    public void ResetHazardObject()
    {
        StopAllCoroutines();
        movingBackwards = false;
        transform.parent.localPosition = StartPos;
    }

    public void UpdateHazard(float hazardOffset, Vector3 moveDirection)
    {
        if (movingBackwards == false)
            MoveHazard(transform.parent.position + hazardOffset * moveDirection);
        else
            MoveHazard(transform.parent.position - hazardOffset * moveDirection);

    }

    public void ReverseHazard(float hazardOffset, Vector3 moveDirection)
    {
        if (movingBackwards == true)
            MoveHazard(transform.parent.position + hazardOffset * moveDirection);
        else
            MoveHazard(transform.parent.position - hazardOffset * moveDirection);
    }
    private void MoveHazard(Vector3 targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(ExecuteMove(targetPosition));
    }

    private IEnumerator ExecuteMove(Vector3 targetPosition)
    {
        float time = 0f;
        while (time < moveTime)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, targetPosition, time * (1 / moveTime));
            time += Time.deltaTime;
            yield return null;
        }
    }
    public void CheckHazardBounds(int boundsMax, Vector3 moveDirection, float hazardOffset)
    {
        CheckParticleBounds(boundsMax, moveDirection, hazardOffset);

        Vector3 vec;
        if (movingBackwards)
            vec = transform.parent.localPosition + (-moveDirection * hazardOffset);
        else
            vec = transform.parent.localPosition + (moveDirection * hazardOffset);


        if (Mathf.Round(vec.x) > boundsMax || Mathf.Round(vec.z) > boundsMax)
        {
            TurnAround();
            return;
        }

        if (Mathf.Round(vec.x) < -boundsMax || Mathf.Round(vec.z) < -boundsMax)
        {
            TurnAround();
        }

    }

    private void CheckParticleBounds(int boundsMax, Vector3 moveDirection, float hazardOffset)
    {
        Vector3 pVec;

        pVec = transform.parent.localPosition + 2 * (direction * hazardOffset);


        if (Mathf.Round(pVec.x) > boundsMax || Mathf.Round(pVec.z) > boundsMax)
        {
            TurnParticlesAround();
            return;
        }

        if (Mathf.Round(pVec.x) < -boundsMax || Mathf.Round(pVec.z) < -boundsMax)
        {
            TurnParticlesAround();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EventHandler<ResetPuzzleEvent>.FireEvent(new ResetPuzzleEvent(new PuzzleInfo(PuzzleID)));
    }

    public void SetDirection(Vector3 vec)
    {
        direction = vec;
        if (lavaParticles != null)
            lavaParticles.SetVector3("NextPos", direction);
    }
    private void TurnAround()
    {
        movingBackwards = !movingBackwards;
    }

    private void TurnParticlesAround()
    {
        direction *= -1;
        if (lavaParticles != null)
            lavaParticles.SetVector3("NextPos", direction);
    }
}
