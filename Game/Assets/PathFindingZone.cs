using Assets.Scripts.Playmode.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindingZone : MonoBehaviour
{
    [SerializeField] public List<PathFindingLine> Lines;

    private Vector2 max;
    private Vector2 min;

    private void Start()
    {
        max = new Vector2(int.MinValue, int.MinValue);
        min = new Vector2(int.MaxValue, int.MaxValue);

        foreach (PathFindingLine line in Lines)
        {
            max.x = Mathf.Max(max.x, line.pointATransform.position.x, line.pointBTransform.position.x);
            max.y = Mathf.Max(max.y, line.pointATransform.position.z, line.pointBTransform.position.z);
            min.x = Mathf.Min(min.x, line.pointATransform.position.x, line.pointBTransform.position.x);
            min.y = Mathf.Min(min.y, line.pointATransform.position.z, line.pointBTransform.position.z);
        }
    }

    public bool IsPointInZone(Vector3 point)
    {
        if (!(point.x > min.x && point.x < max.x && point.z > min.y && point.z < max.y))
            return false;

        point.y = 0;

        float? sign = null;
        foreach(PathFindingLine pathFindingLine in Lines)
        {
            Vector3 vector = point - pathFindingLine.line.pointA;

            Debug.DrawLine(point, pathFindingLine.line.pointA, Color.red);

            float crossProductHeight = Vector3.Cross(pathFindingLine.line.pointB - pathFindingLine.line.pointA, vector).y;

            if (sign == null)
                sign = Mathf.Sign(crossProductHeight);

            if (Mathf.Sign(crossProductHeight) != sign)
                return false;
        }

        return true;
    }
}
