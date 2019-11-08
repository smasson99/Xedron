using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Vector3 point;

    private void Awake()
    {
        Initialize(this.transform.position);
    }

    public void Initialize(Vector3 point)
    {
        point.y = 0;
        this.point = point;
    }
}

