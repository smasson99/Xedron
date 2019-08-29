using Assets.Scripts.Playmode.Utils;
using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LineRenderer lineRenderer;

    private List<Node> closedList = new List<Node>();
    private List<Node> openList = new List<Node>();
    private List<Vector3> points = new List<Vector3>();

    private void Update()
    {
        UpdatePath(transform.position, target.position);
    }

    private void UpdatePath(Vector3 subjectPosition, Vector3 targetPosition)
    {
        ClearPath();

        subjectPosition.y = 0;
        targetPosition.y = 0;

        PathFindingZone zoneObjectIsIn, zoneTargetIsIn;
        FindTargetAndObjectZone(out zoneObjectIsIn, out zoneTargetIsIn);

        if (zoneObjectIsIn == null || zoneTargetIsIn == null)
        {
            PathNotFound();
            return;
        }
        else
        {
            Debug.Log($"zoneObjectIsIn: {zoneObjectIsIn}, zoneTargetIsIn: {zoneTargetIsIn}");
        }

        if (zoneObjectIsIn == zoneTargetIsIn)
        {
            FoundPath(subjectPosition, targetPosition, points, null);
            return;
        }

        foreach (PathFindingLine pathFindingLine in zoneObjectIsIn.Lines)
        {
            if (pathFindingLine.zone != null)
                openList.Add(new Node() { Parent = null, Line = pathFindingLine, Score = Node.CalculateScore(pathFindingLine.line, subjectPosition, targetPosition) });
        }

        int count = 0;
        while (openList.Count > 0 && count < 100)
        {
            Node min = openList.First();
            foreach (Node node in openList)
            {
                if (min.Score > node.Score)
                    min = node;
            }

            if (min.Line.zone == zoneTargetIsIn)
            {
                FoundPath(subjectPosition, targetPosition, points, min);
                return;
            }

            closedList.Add(min);
            openList.Remove(min);

            foreach (PathFindingLine pathFindingLine in min.Line.zone.Lines)
            {
                if (pathFindingLine.zone != null)
                {
                    float score = Node.CalculateScore(pathFindingLine.line, targetPosition, subjectPosition);

                    Node existingNode = openList.Find(x => x.Line == pathFindingLine);
                    if (existingNode == null)
                        existingNode = closedList.Find(x => x.Line == pathFindingLine);

                    if (existingNode != null)
                    {
                        if (existingNode.Score > score)
                        {
                            existingNode.Parent = min;
                            existingNode.Score = score;
                        }
                    }
                    else
                    {
                        openList.Add(new Node() { Parent = min, Line = pathFindingLine, Score = score });
                    }
                }
            }

            count++;
        }

        PathNotFound();
    }

    private void ClearPath()
    {
        closedList.Clear();
        openList.Clear();
        points.Clear();
    }

    private void PathNotFound()
    {
        lineRenderer.enabled = false;
        Debug.Log("Not Found");
    }

    private void FindTargetAndObjectZone(out PathFindingZone zoneObjectIsIn, out PathFindingZone zoneTargetIsIn)
    {
        zoneObjectIsIn = null;
        zoneTargetIsIn = null;
        GameObject[] pathFindingZones = GameObject.FindGameObjectsWithTag(R.S.Tag.PathFindingZone);
        foreach (GameObject gameObject in pathFindingZones)
        {
            PathFindingZone pathFindingZone;
            if ((pathFindingZone = gameObject.GetComponent<PathFindingZone>()) && zoneObjectIsIn == null && (pathFindingZone?.IsPointInZone(transform.position) ?? false))
            {
                zoneObjectIsIn = pathFindingZone;
                if (zoneTargetIsIn != null)
                    break;
            }

            if (pathFindingZone != null && zoneTargetIsIn == null && (pathFindingZone?.IsPointInZone(target.position) ?? false))
            {
                zoneTargetIsIn = pathFindingZone;
                if (zoneObjectIsIn != null)
                    break;
            }
        }
    }

    private void FoundPath(Vector3 positionCurrent, Vector3 positionTarget, List<Vector3> points, Node min)
    {
        SliderNode first = new SliderNode(positionTarget, null, null);
        SliderNode current = first;

        Debug.Log("Found");
        Node nextNode = min;

        while (nextNode != null)
        {
            current.next = new SliderNode(nextNode.Line, current, null);
            current = current.next;

            nextNode = nextNode.Parent;
        }

        current.next = new SliderNode(positionCurrent, current, null);

        SliderNode.optimzeCount = 0;
        current.Optimize(positionTarget);
        Debug.Log($"Current function call of smothing: {SliderNode.optimzeCount}");

        current = current.next;

        while (current != null)
        {
            points.Add(current.point);
            current = current.previous;
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }

    

    private class Node
    {
        public Node Parent { get; set; }
        public PathFindingLine Line { get; set; }
        public float Score { get; set; }

        public static float CalculateScore(Line line, Vector3 subjectPosition, Vector3 targetPosition)
        {
            return (line.center - subjectPosition).magnitude + (targetPosition - line.center).magnitude;
        }
    }

    public class SliderNode
    {
        public static int optimzeCount = 0;

        public Vector3 point;
        public PathFindingLine line;
        public SliderNode previous;
        public SliderNode next;

        private bool HasBeenOptimize = false;
        private bool flatLine = false;

        public SliderNode(Vector3 point, SliderNode previous, SliderNode next)
        {
            this.previous = previous;
            this.next = next;
            this.point = point;
        }

        public SliderNode(PathFindingLine line, SliderNode previous, SliderNode next)
        {
            this.previous = previous;
            this.next = next;
            this.line = line;
            point = line.line.center;
        }

        public void Optimize(Vector3 target)
        {
            optimzeCount++;
            HasBeenOptimize = true;

            if (optimzeCount >= 120)
                throw new Exception("Optmize path: Might be in an infinite loop");

            if(previous != null && next != null)
            {
                Vector3 pointBeforeOptimze = point;

                SliderNode nextNonFlat = next;
                SliderNode previousNonFlat = previous;
                while(nextNonFlat != null)
                {
                    if(!nextNonFlat.flatLine || nextNonFlat.point.IsEquals(nextNonFlat.next.point))
                        break;

                    nextNonFlat = nextNonFlat.next;
                }

                while (previousNonFlat != null)
                {
                    if (!previousNonFlat.flatLine || previousNonFlat.point.IsEquals(previousNonFlat.previous.point))
                        break;

                    previousNonFlat = previousNonFlat.previous;
                }

                Line lineBetweenNextPrevious = new Line(nextNonFlat.point, previousNonFlat.point);

                Vector3? intersectPoint = lineBetweenNextPrevious.IntersectionPoint(line.line);

                if(intersectPoint.HasValue && line.line.IsInRange(intersectPoint.Value) && lineBetweenNextPrevious.IsInRange(intersectPoint.Value))
                {
                    flatLine = true;

                    point = intersectPoint.Value;
                }
                else if (!intersectPoint.HasValue)
                {
                    flatLine = true;
                }
                else
                {
                    flatLine = false;

                    if (((previousNonFlat.point - line.line.pointA).magnitude + (line.line.pointA - nextNonFlat.point).magnitude) < ((previousNonFlat.point - line.line.pointB).magnitude + (line.line.pointB - nextNonFlat.point).magnitude))
                        point = line.line.pointA;
                    else
                        point = line.line.pointB;
                }

                if (!pointBeforeOptimze.IsEquals(point))
                {
                    previous.Optimize(target);
                    next.Optimize(target);
                }
                else 
                {
                    if (!previous.HasBeenOptimize)
                        previous.Optimize(target);
                    if (!next.HasBeenOptimize)
                        next.Optimize(target);
                }
            }
        }
    }
}




