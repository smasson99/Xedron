using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTreeRandomIndexGenerator : StateMachineBehaviour
{
    [SerializeField] private string blendTreeIndexParameterName = "ParameterName";
    [SerializeField] private int maxIndexValue;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(blendTreeIndexParameterName, Random.Range(1, maxIndexValue + 1));
    }
}