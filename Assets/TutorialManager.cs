using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] int numberOfSteps;
    [SerializeField] int stepIndex = 1;

    [Header("Component References")]
    [SerializeField] Animator anim;

    private void Start()
    {
        anim.Play("Move" + stepIndex);
    }
    public void NextStep()
    {
        if(stepIndex<5)
        anim.gameObject.SetActive(false);
        stepIndex++;
        if (stepIndex <= numberOfSteps)
        {
            anim.gameObject.SetActive(true);
            anim.Play("Move" + stepIndex);
        }

    }
}
