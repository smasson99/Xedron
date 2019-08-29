using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathFindingObstacle : MonoBehaviour
{
    public List<PathFindingLine> lines = new List<PathFindingLine>();
    public List<Vector3> points = new List<Vector3>();
    public bool Clockwise = false;

    public void Initialize()
    {
        points.Clear();
        foreach (PathFindingLine line in lines)
        {
            if (points.FindIndex(x => x.IsEquals(line.pointATransform.position)) == -1)
                points.Add(line.pointATransform.position);

            if (points.FindIndex(x => x.IsEquals(line.pointBTransform.position)) == -1)
                points.Add(line.pointBTransform.position);
        }
    }

    private void Start()
    {
        Initialize();

        foreach (PathFindingLine line in lines)
            line.isObstacleLine = this;
    }

    private void Awake()
    {
        Initialize();
    }
}
