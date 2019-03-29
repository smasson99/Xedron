using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageOnStateEnter : StateMachineBehaviour
{
    private const string DefaultFunctionName = "Function Name";

    [SerializeField] private string functionName = DefaultFunctionName;
    
    private void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.transform.root.gameObject.SendMessage(functionName);
    }
}
