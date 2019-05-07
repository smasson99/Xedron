using Harmony;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPillar : MonoBehaviour
{
    [SerializeField] private GameObject pillarPref;
    [SerializeField] private Transform target;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, 1 << LayerMask.NameToLayer(R.S.Layer.Default)))
            {
                Pillar pillar = Instantiate(pillarPref).GetComponent<Pillar>();

                pillar.Initialize(hit.point, target.position);

                pillar.Raise();
            }
        }
    }
}
