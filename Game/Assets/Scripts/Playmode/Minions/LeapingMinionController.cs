using Harmony;
using System.Linq;
using UnityEngine;

public class LeapingMinionController : MonoBehaviour
{
    [SerializeField] private Transform target;

    private LeapingMinion[] leapingMinions;

    private void Awake()
    {
        leapingMinions = GameObject.FindGameObjectsWithTag(R.S.Tag.Minion).Select(minionObject => minionObject.GetComponent<LeapingMinion>()).ToArray();
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            foreach (LeapingMinion leapingMinion in leapingMinions)
                leapingMinion.Leap(target.position, 5);
        }
    }
}
