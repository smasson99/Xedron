using Assets.Scripts.Playmode.Utils;
using Harmony;
using UnityEngine;

[ExecuteInEditMode]
public class PathFindingLine : MonoBehaviour
{
    [SerializeField] public Transform pointATransform;
    [SerializeField] public Transform pointBTransform;

    [SerializeField] public PathFindingZone zone;
    public Line line;
    public bool generatedLine = false;
    public PathFindingObstacle isObstacleLine;
    public PathFindingZone owner;

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        line = new Line(pointATransform.position, pointBTransform.position) { pathFindingLine = this, zone = zone};
        transform.position = line.center;
    }

    private void Update()
    {
        Debug.DrawLine(pointATransform.position, pointBTransform.position, isObstacleLine ? Color.red : CompareTag(R.S.Tag.PathFindingBoundingLine) ? Color.magenta : Color.blue);
    }

    public void Reverse()
    {
        Transform temp = pointATransform;
        pointATransform = pointBTransform;
        pointBTransform = temp;

        Initialize();
    }
}
