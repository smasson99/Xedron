using Assets.Scripts.Playmode.Utils;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

class GeneratePathFindingObject
{
    [MenuItem("Tools/PathFinding/Generate")]
    private static void GeneratePathFindingZones()
    {
        int countLineAdded = 0;

        List<PathFindingObstacle> obstacles = GameObject.FindGameObjectsWithTag(R.S.Tag.PathFindingZoneObstacle).Select(x => x.GetComponent<PathFindingObstacle>()).ToList();
        List<PathFindingLine> boundingLine = GameObject.FindGameObjectsWithTag(R.S.Tag.PathFindingBoundingLine).Select(x => x.GetComponent<PathFindingLine>()).ToList();
        List<Vector3> boundingPoints = new List<Vector3>();
        List<Vector3> allPoints = new List<Vector3>();
        List<PathFindingLine> allLines = new List<PathFindingLine>();
        List<Transform> allPointsTransform = GameObject.FindGameObjectsWithTag(R.S.Tag.PathFindingPoint).Select(x => x.transform).ToList();
        allLines.AddRange(boundingLine);

        foreach (PathFindingObstacle obstacle in obstacles)
        {
            for (int i = 0; i < obstacle.points.Count; i++)
            {
                for (int j = i + 1; j < obstacle.points.Count; j++)
                {
                    Line lineToAnalyse = new Line(obstacle.points[i], obstacle.points[j]);

                    int? resultPoint = null;
                    bool allPointsSameSide = true;
                    foreach (Vector3 point in obstacle.points)
                    {
                        if (!point.IsEquals(lineToAnalyse.pointA) && !point.IsEquals(lineToAnalyse.pointB))
                        {
                            int resultCrossProduct = Math.Sign(Vector3.Cross(lineToAnalyse.vector, point - lineToAnalyse.pointA).y);

                            if (resultPoint == null)
                                resultPoint = resultCrossProduct;

                            if (resultPoint != resultCrossProduct)
                            {
                                allPointsSameSide = false;
                                break;
                            }
                        }
                    }

                    if (allPointsSameSide)
                    {
                        bool lineAlreadyExist = false;
                        foreach (PathFindingLine line in obstacle.lines)
                        {
                            if ((lineToAnalyse.pointA.IsEquals(line.line.pointA) || lineToAnalyse.pointA.IsEquals(line.line.pointB)) && (lineToAnalyse.pointB.IsEquals(line.line.pointA) || lineToAnalyse.pointB.IsEquals(line.line.pointB)))
                            {
                                lineAlreadyExist = true;
                                break;
                            }
                        }

                        if (!lineAlreadyExist)
                        {
                            Debug.DrawLine(lineToAnalyse.pointA, lineToAnalyse.pointB, Color.cyan);

                            GameObject gameObject = new GameObject();
                            gameObject.name = countLineAdded.ToString();
                            PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                            Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(lineToAnalyse.pointA));
                            Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(lineToAnalyse.pointB));

                            pathFindingLine.pointATransform = pointA;
                            pathFindingLine.pointBTransform = pointB;
                            pathFindingLine.generatedLine = true;

                            pathFindingLine.Initialize();

                            countLineAdded++;

                            allLines.Add(pathFindingLine);
                        }
                    }
                }
            }

            obstacle.lines.ForEach(x => x.Initialize());
            obstacle.Initialize();
            allLines.AddRange(obstacle.lines);
        }

        foreach (PathFindingLine line in boundingLine)
        {
            if (boundingPoints.FindIndex(x => x.IsEquals(line.pointATransform.position)) == -1)
                boundingPoints.Add(line.pointATransform.position);

            if (boundingPoints.FindIndex(x => x.IsEquals(line.pointBTransform.position)) == -1)
                boundingPoints.Add(line.pointBTransform.position);
        }

        foreach (PathFindingObstacle obstacle in obstacles)
            allPoints.AddRange(obstacle.points);

        allPoints.AddRange(boundingPoints);

        foreach (PathFindingObstacle obstacle in obstacles)
        {
            int signCrossProductInside = obstacle.Clockwise ? -1 : 1;

            foreach (Vector3 point in obstacle.points)
            {
                Vector3 tempcochonerie = point;

                List<Vector3?> visiblesPoints = new List<Vector3?>();
                List<Line> linesLinkToPoint = obstacle.lines.Select(x => x.line).Where(x => x.pointA.IsEquals(point) || x.pointB.IsEquals(point)).ToList();
                if (linesLinkToPoint.Count != 2)
                    throw new Exception(point.ToString());
                foreach (Vector3 pointToAnalyse in allPoints)
                {
                    if (!pointToAnalyse.IsEquals(linesLinkToPoint[0].pointA) && !pointToAnalyse.IsEquals(linesLinkToPoint[0].pointB) && !pointToAnalyse.IsEquals(linesLinkToPoint[1].pointA) && !pointToAnalyse.IsEquals(linesLinkToPoint[1].pointB))
                    {
                        bool isPointVisible = true;

                        Line lineBetweenPointAndPointToAnalyze = new Line(point, pointToAnalyse);

                        foreach (PathFindingLine line in allLines)
                        {
                            Vector3? intersectionPoint = line.line.IntersectionPoint(lineBetweenPointAndPointToAnalyze);

                            if (intersectionPoint != null && !intersectionPoint.Value.IsEquals(point) && !intersectionPoint.Value.IsEquals(pointToAnalyse) && line.line.IsInRange(intersectionPoint.Value) && lineBetweenPointAndPointToAnalyze.IsInRange(intersectionPoint.Value))
                            {
                                isPointVisible = false;
                                break;
                            }

                        }

                        if (isPointVisible)
                        {
                            Line line = new Line(point, pointToAnalyse);
                            int crossProductBetweenLine;
                            if (linesLinkToPoint[0].pointB.IsEquals(linesLinkToPoint[1].pointA))
                            {
                                crossProductBetweenLine = Math.Sign(Vector3.Cross(linesLinkToPoint[0].vector, linesLinkToPoint[1].vector).y);
                            }
                            else
                            {
                                crossProductBetweenLine = Math.Sign(Vector3.Cross(linesLinkToPoint[1].vector, linesLinkToPoint[0].vector).y);
                            }

                            int crossProduct1 = Math.Sign(Vector3.Cross(line.vector, linesLinkToPoint[0].vector).y);
                            int crossProduct2 = Math.Sign(Vector3.Cross(line.vector, linesLinkToPoint[1].vector).y);

                            if (crossProductBetweenLine != signCrossProductInside && (crossProduct1 != signCrossProductInside || crossProduct2 != signCrossProductInside))
                            {
                                visiblesPoints.Add(pointToAnalyse);
                                Debug.DrawLine(point, pointToAnalyse, Color.magenta, 3);
                            }
                        }
                    }
                }


                List<Vector3?> prefectPoints = visiblesPoints.Select(x => x).Where(delegate (Vector3? point2)
                {
                    Line line = new Line(point, point2.Value);
                    int crossProduct1 = Math.Sign(Vector3.Cross(line.vector, linesLinkToPoint[0].vector).y);
                    int crossProduct2 = Math.Sign(Vector3.Cross(line.vector, linesLinkToPoint[1].vector).y);
                    return crossProduct1 != signCrossProductInside && crossProduct2 != signCrossProductInside;
                }).ToList();

                prefectPoints.Sort(delegate (Vector3? point1, Vector3? point2)
                {
                    return (int)((point - point1).Value.sqrMagnitude - (point - point2).Value.sqrMagnitude);
                });

                if (prefectPoints.Count > 0)
                {
                    Vector3? pointToUse = prefectPoints.First();

                    bool lineExist = false;
                    foreach (PathFindingLine line in allLines)
                    {
                        if ((line.line.pointA.IsEquals(pointToUse.Value) || line.line.pointB.IsEquals(pointToUse.Value))
                            && (line.line.pointA.IsEquals(point) || line.line.pointB.IsEquals(point)))
                        {
                            lineExist = true;
                            break;
                        }
                    }

                    if (!lineExist)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = countLineAdded.ToString();
                        PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                        Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(point));
                        Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(pointToUse.Value));

                        pathFindingLine.pointATransform = pointA;
                        pathFindingLine.pointBTransform = pointB;
                        pathFindingLine.generatedLine = true;

                        pathFindingLine.Initialize();

                        countLineAdded++;

                        allLines.Add(pathFindingLine);
                    }
                }
                else
                {
                    List<Vector3?> pointsPositive = new List<Vector3?>();
                    List<Vector3?> pointsNegative = new List<Vector3?>();

                    visiblesPoints.ForEach(delegate (Vector3? point2)
                    {
                        Line line = new Line(point, point2.Value);
                        int crossProduct1 = Math.Sign(Vector3.Cross(line.vector, linesLinkToPoint[0].vector).y);

                        if (crossProduct1 == 1)
                            pointsPositive.Add(point2);
                        else
                            pointsNegative.Add(point2);
                    });

                    Vector3? firstPointForLine = null;
                    Vector3? secondPointForLine = null;

                    Line referenceLine = linesLinkToPoint[0];

                    if (linesLinkToPoint[0].pointA != point)
                        referenceLine = linesLinkToPoint[0].Reverse();

                    foreach (Vector3 point2 in pointsPositive)
                    {
                        Line line = new Line(point, point2);
                        int crossProduct1 = Math.Sign(Vector3.Cross(line.vector, referenceLine.vector).y);


                        foreach (Vector3 point3 in pointsNegative)
                        {
                            Line line2 = new Line(point, point3);
                            int crossProduct2 = Math.Sign(Vector3.Cross(line.vector, line2.vector).y);

                            if (crossProduct1 != crossProduct2)
                            {
                                firstPointForLine = point2;
                                secondPointForLine = point3;
                                break;
                            }

                            if (firstPointForLine != null)
                                break;
                        }
                    }

                    if (firstPointForLine != null)
                    {
                        {
                            bool lineExist = false;
                            foreach (PathFindingLine line in allLines)
                            {
                                if ((line.line.pointA.IsEquals(firstPointForLine.Value) || line.line.pointB.IsEquals(firstPointForLine.Value))
                                    && (line.line.pointA.IsEquals(point) || line.line.pointB.IsEquals(point)))
                                {
                                    lineExist = true;
                                    break;
                                }
                            }

                            if (!lineExist)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.name = countLineAdded.ToString();
                                PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                                Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(point));
                                Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(firstPointForLine.Value));

                                pathFindingLine.pointATransform = pointA;
                                pathFindingLine.pointBTransform = pointB;
                                pathFindingLine.generatedLine = true;

                                pathFindingLine.Initialize();

                                countLineAdded++;

                                allLines.Add(pathFindingLine);
                            }
                        }

                        {
                            bool lineExist = false;
                            foreach (PathFindingLine line in allLines)
                            {
                                if ((line.line.pointA.IsEquals(secondPointForLine.Value) || line.line.pointB.IsEquals(secondPointForLine.Value))
                                    && (line.line.pointA.IsEquals(point) || line.line.pointB.IsEquals(point)))
                                {
                                    lineExist = true;
                                    break;
                                }
                            }
                            if (!lineExist)
                            {
                                GameObject gameObject = new GameObject();
                                gameObject.name = countLineAdded.ToString();
                                PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                                Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(point));
                                Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(secondPointForLine.Value));

                                pathFindingLine.pointATransform = pointA;
                                pathFindingLine.pointBTransform = pointB;
                                pathFindingLine.generatedLine = true;

                                pathFindingLine.Initialize();

                                countLineAdded++;

                                allLines.Add(pathFindingLine);
                            }
                        }
                    }
                    else
                    {
                        //throw new Exception();
                    }
                }
            }
        }

        CreateZones(countLineAdded, obstacles, allLines, allPointsTransform);
    }

    private static int CreateZones(int countLineAdded, List<PathFindingObstacle> obstacles, List<PathFindingLine> allLines, List<Transform> allPointsTransform)
    {
        List<PathFindingZone> zones = new List<PathFindingZone>();
        int currentZoneNumber = 0;
        int count = 0;
        while (allLines.Count > 0)
        {
            count++;
            currentZoneNumber++;

            GameObject gameObjectZone = new GameObject();
            gameObjectZone.name = "PathFindingZone (" + currentZoneNumber + ")";
            gameObjectZone.tag = R.S.Tag.PathFindingZone;
            PathFindingZone currentZone = gameObjectZone.AddComponent<PathFindingZone>();

            PathFindingLine currentLine = allLines.First();
            allLines.Remove(currentLine);

            currentZone.Lines = new List<PathFindingLine>();

            {
                PathFindingObstacle isAnObstaclesLine = null;
                if (currentLine != null)
                {
                    foreach (PathFindingObstacle obstacle in obstacles)
                    {
                        foreach (PathFindingLine lineObstacles in obstacle.lines)
                        {
                            if (lineObstacles == currentLine)
                            {
                                isAnObstaclesLine = obstacle;
                                break;
                            }
                        }

                        if (isAnObstaclesLine)
                            break;
                    }
                }

                if (isAnObstaclesLine)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = countLineAdded.ToString();
                    PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                    Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(currentLine.line.pointA));
                    Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(currentLine.line.pointB));

                    pathFindingLine.pointATransform = pointA;
                    pathFindingLine.pointBTransform = pointB;
                    pathFindingLine.generatedLine = true;
                    pathFindingLine.isObstacleLine = isAnObstaclesLine;
                    pathFindingLine.owner = currentZone;

                    pathFindingLine.Initialize();

                    currentLine = pathFindingLine;

                    countLineAdded++;
                }
            }


            if (currentLine.generatedLine)
                currentLine.transform.parent = currentZone.transform;

            currentZone.Lines.Add(currentLine);
            currentLine.owner = currentZone;

            zones.Add(currentZone);

            while ((currentZone.Lines.Count == 0 || !currentZone.Lines.First().line.pointA.IsEquals(currentLine.line.pointB)))
            {
                count++;

                if (count >= 200)
                    throw new Exception();

                string temp = currentLine.ToString();
                Debug.Log("Current: " + currentLine);

                List<Line> lines = new List<Line>();
                foreach (PathFindingLine line in allLines)
                {
                    if (line.line.pointA.IsEquals(currentLine.line.pointB))
                    {
                        lines.Add(line.line);
                    }
                    else if (line.line.pointB.IsEquals(currentLine.line.pointB))
                    {
                        lines.Add(line.line.Reverse());
                    }
                }
                foreach (PathFindingZone zone in zones)
                {
                    foreach (PathFindingLine line in zone.Lines)
                    {
                        bool lineAlreadyAdd = false;
                        foreach(PathFindingLine lineMightAlreadyAdded in currentZone.Lines)
                        {
                            if((lineMightAlreadyAdded.line.pointA.IsEquals(line.line.pointA) && lineMightAlreadyAdded.line.pointB.IsEquals(line.line.pointB)) || (lineMightAlreadyAdded.line.pointA.IsEquals(line.line.pointB) && lineMightAlreadyAdded.line.pointB.IsEquals(line.line.pointA)))
                            {
                                lineAlreadyAdd = true;
                                break;
                            }
                        }

                        if(!lineAlreadyAdd)
                        {
                            if (line != currentLine && line.line.pointA.IsEquals(currentLine.line.pointB) && !line.line.pointB.IsEquals(currentLine.line.pointA))
                            {
                                lines.Add(line.line);
                            }
                            else if (line != currentLine && line.line.pointB.IsEquals(currentLine.line.pointB) && !line.line.pointA.IsEquals(currentLine.line.pointB))
                            {
                                lines.Add(line.line.Reverse());
                            }
                        }
                    }
                }

                lines.Sort(delegate (Line x, Line y)
                {
                    Line reverseCurrentLine = currentLine.line.Reverse();

                    float angleBetween1 = x.AngleBetweenLine(reverseCurrentLine);
                    float angleBetween2 = y.AngleBetweenLine(reverseCurrentLine);
                    return (int)(angleBetween1 - angleBetween2);
                });

                foreach (Line line in lines)
                {
                    bool isTowardsInside = true;

                    PathFindingObstacle isAnObstaclesLine = null, isPreviousAnObstaclesLine = null;

                    if (currentZone.Lines.Count >= 1 && currentZone.Lines[currentZone.Lines.Count - 1].isObstacleLine)
                        isPreviousAnObstaclesLine = currentZone.Lines[currentZone.Lines.Count - 1].isObstacleLine;
                    if (line.pathFindingLine.isObstacleLine)
                        isAnObstaclesLine = line.pathFindingLine.isObstacleLine;

                    bool isAroundObstacle = false;
                    if (isPreviousAnObstaclesLine == isAnObstaclesLine && isPreviousAnObstaclesLine != null)
                    {
                        Line lineToAnalyze = line;

                        Line lineToCheck = currentZone.Lines[currentZone.Lines.Count - 1].line;

                        foreach (PathFindingLine lineInObstacles in isPreviousAnObstaclesLine.lines)
                        {
                            if ((lineToCheck.pointA.IsEquals(lineInObstacles.line.pointA) || lineToCheck.pointA.IsEquals(lineInObstacles.line.pointB)) && (lineToCheck.pointB.IsEquals(lineInObstacles.line.pointA) || lineToCheck.pointB.IsEquals(lineInObstacles.line.pointB)))
                            {
                                lineToCheck = new Line(lineInObstacles.line.pointA, lineInObstacles.line.pointB);
                            }

                            if((lineToAnalyze.pointA.IsEquals(lineInObstacles.line.pointA) || lineToAnalyze.pointA.IsEquals(lineInObstacles.line.pointB)) && (lineToAnalyze.pointB.IsEquals(lineInObstacles.line.pointA) || lineToAnalyze.pointB.IsEquals(lineInObstacles.line.pointB)))
                            {
                                lineToAnalyze = new Line(lineInObstacles.line.pointA, lineInObstacles.line.pointB);
                            }
                        }

                        int signCrossProductInside = isPreviousAnObstaclesLine.Clockwise ? -1 : 1;

                        int crossProductBetweenLine;
                        if (lineToCheck.pointB.IsEquals(lineToAnalyze.pointA))
                        {
                            crossProductBetweenLine = Math.Sign(Vector3.Cross(lineToCheck.vector, lineToAnalyze.vector).y);
                        }
                        else
                        {
                            crossProductBetweenLine = Math.Sign(Vector3.Cross(lineToAnalyze.vector, lineToCheck.vector).y);
                        }

                        if (crossProductBetweenLine != signCrossProductInside)
                            isAroundObstacle = true;
                    }


                    //Check if is gonna be convex shape
                    if (currentZone.Lines.Count >= 2)
                    {
                        string temp2 = line.pathFindingLine.ToString();

                        Line lineToEvaluate = line;

                        if (!lineToEvaluate.pointA.IsEquals(currentLine.line.pointB))
                            lineToEvaluate = lineToEvaluate.Reverse();

                        int crossProduct = Math.Sign(Vector3.Cross(currentLine.line.vector, lineToEvaluate.vector).y);
                        int crossProductInside = Math.Sign(Vector3.Cross(currentZone.Lines[0].line.vector, currentZone.Lines[1].line.vector).y);

                        if (crossProduct != crossProductInside && crossProduct != 0)
                            isTowardsInside = false;
                    }

                    if (isTowardsInside && !isAroundObstacle)
                    {
                        PathFindingZone isLineInZone = null;
                        foreach (PathFindingZone zone in zones)
                        {
                            foreach (PathFindingLine lineInZone in zone.Lines)
                            {
                                if (lineInZone.pointATransform.position.IsEquals(line.pointA) && lineInZone.pointBTransform.position.IsEquals(line.pointB)
                                    || lineInZone.pointBTransform.position.IsEquals(line.pointA) && lineInZone.pointATransform.position.IsEquals(line.pointB))
                                {
                                    lineInZone.zone = currentZone;
                                    lineInZone.line.zone = currentZone;

                                    isLineInZone = zone;
                                    break;
                                }
                            }

                            if (isLineInZone)
                                break;
                        }

                        PathFindingLine lineToAdd = line.pathFindingLine;
                        allLines.Remove(lineToAdd);


                        if (lineToAdd == null || isAnObstaclesLine || (isLineInZone != null))
                        {
                            GameObject gameObject = new GameObject();
                            gameObject.name = countLineAdded.ToString();
                            PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                            Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(line.pointA));
                            Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(line.pointB));

                            pathFindingLine.pointATransform = pointA;
                            pathFindingLine.pointBTransform = pointB;
                            pathFindingLine.zone = isLineInZone;
                            pathFindingLine.generatedLine = true;
                            if (isAnObstaclesLine != null)
                                pathFindingLine.isObstacleLine = isAnObstaclesLine;

                            pathFindingLine.Initialize();

                            lineToAdd = pathFindingLine;

                            countLineAdded++;
                        }

                        if (!line.pointA.IsEquals(lineToAdd.pointATransform.position))
                        {
                            lineToAdd.Reverse();
                        }

                        if (lineToAdd.generatedLine)
                            lineToAdd.transform.parent = currentZone.transform;


                        currentZone.Lines.Add(lineToAdd);
                        lineToAdd.owner = currentZone;
                        currentLine = lineToAdd;
                        break;
                    }
                }
            }

            Debug.Log("Zone completed ---------------------------------------------------------------------------------------------------");
            currentZone.Lines.ForEach(x => Debug.Log(x));
        }

        //fill hole

        List<PathFindingLine> lineWithoutSecondZone = new List<PathFindingLine>();

        foreach(PathFindingZone zone in zones)
        {
            foreach(PathFindingLine line in zone.Lines)
            {
                if(!line.isObstacleLine && !line.CompareTag(R.S.Tag.PathFindingBoundingLine) && line.zone == null)
                {
                    lineWithoutSecondZone.Add(line);
                }
            }
        }

        while (lineWithoutSecondZone.Count > 0)
        {
            count++;
            currentZoneNumber++;

            GameObject gameObjectZone = new GameObject();
            gameObjectZone.name = "PathFindingZone (" + currentZoneNumber + ")";
            gameObjectZone.tag = R.S.Tag.PathFindingZone;
            PathFindingZone currentZone = gameObjectZone.AddComponent<PathFindingZone>();
            currentZone.Lines = new List<PathFindingLine>();

            PathFindingLine currentLine = lineWithoutSecondZone.First();
            lineWithoutSecondZone.Remove(currentLine);

            {
                GameObject gameObject = new GameObject();
                gameObject.name = countLineAdded.ToString();
                PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(currentLine.line.pointA));
                Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(currentLine.line.pointB));

                pathFindingLine.pointATransform = pointA;
                pathFindingLine.pointBTransform = pointB;
                pathFindingLine.zone = currentLine.owner;
                pathFindingLine.transform.parent = currentZone.transform;
                pathFindingLine.generatedLine = true;

                pathFindingLine.Initialize();

                countLineAdded++;

                currentLine.zone = currentZone;
                currentLine = pathFindingLine;
                currentLine.transform.parent = currentZone.transform;
                currentLine.owner = currentZone;
            }

            currentZone.Lines.Add(currentLine);

            zones.Add(currentZone);

            while (!currentZone.Lines.First().line.pointA.IsEquals(currentLine.line.pointB))
            {
                List<Line> lines = new List<Line>();
                foreach (PathFindingLine line in lineWithoutSecondZone)
                {
                    PathFindingLine nextLine = null;

                    if (line.line.pointA.IsEquals(currentLine.line.pointB))
                    {
                        nextLine = line;
                        lines.Add(line.line);
                    }
                    else if (line.line.pointB.IsEquals(currentLine.line.pointB))
                    {
                        nextLine = line;
                        lines.Add(line.line.Reverse());
                    }


                    if (nextLine != null)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = countLineAdded.ToString();
                        PathFindingLine pathFindingLine = gameObject.AddComponent<PathFindingLine>();

                        Transform pointA = allPointsTransform.First(x => x.transform.position.IsEquals(line.line.pointA));
                        Transform pointB = allPointsTransform.First(x => x.transform.position.IsEquals(line.line.pointB));

                        pathFindingLine.pointATransform = pointA;
                        pathFindingLine.pointBTransform = pointB;
                        pathFindingLine.zone = line.owner;
                        pathFindingLine.transform.parent = currentZone.transform;
                        pathFindingLine.generatedLine = true;

                        pathFindingLine.Initialize();

                        countLineAdded++;

                        lineWithoutSecondZone.Remove(nextLine);
                        nextLine.zone = currentZone;

                        currentLine = pathFindingLine;
                        currentLine.owner = currentZone;
                        currentZone.Lines.Add(currentLine);

                        break;
                    }
                }
            }
        }

        return countLineAdded;
    }
}

