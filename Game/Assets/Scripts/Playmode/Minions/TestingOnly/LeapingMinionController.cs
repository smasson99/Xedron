using System.Linq;
using Harmony;
using UnityEngine;

namespace Game
{
    public class LeapingMinionController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private Leaper[] leapers;

        private void Awake()
        {
            leapers = GameObject.FindGameObjectsWithTag(R.S.Tag.Minion).Select(minionObject => minionObject.GetComponent<Leaper>()).ToArray();
        }

        void Update()
        {
            if(Input.anyKeyDown)
            {
                foreach (Leaper leapingMinion in leapers)
                    leapingMinion.Leap(target.position, 5);
            }
        }
    }
}
