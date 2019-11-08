using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Playmode.Utils
{
    public class Line
    {
        public static float epsilon = 0.01f;

        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 vector;
        public Vector3 center;

        public float a;
        public float b;
        //Vertical
        public float? x;

        public Vector3 max;
        public Vector3 min;
        public float magnitude => vector.magnitude;

        public PathFindingLine pathFindingLine;
        public PathFindingZone zone;

        public Line(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;

            InitializeLineEquation();

            max = new Vector3(Mathf.Max(pointA.x, pointB.x), 0, Mathf.Max(pointA.z, pointB.z));
            min = new Vector3(Mathf.Min(pointA.x, pointB.x), 0, Mathf.Min(pointA.z, pointB.z));

            center = (pointB - pointA) * 0.5f + pointA;
            vector = pointB - pointA;
        }

        public void InitializeLineEquation()
        {
            float deltaX = (pointA.x - pointB.x);
            float deltaY = (pointA.z - pointB.z);

            if (Mathf.Abs(deltaX) < float.Epsilon)
            {
                x = pointA.x;
                a = 0;
                b = 0;
                return;
            }

            a = deltaY / deltaX;
            b = -a * pointA.x + pointA.z;
            x = null;
        }


        public Line Reverse()
        {
            return new Line(pointB, pointA) { pathFindingLine = this.pathFindingLine, zone = this.zone };
        }

        public Line GetPerpendicularLine()
        {
            return new Line(new Vector3(-(pointA - center).z, 0, (pointA - center).x) + center, new Vector3(-(pointB - center).z, 0, (pointB - center).x) + center);
        }

        public float AngleBetweenLine(Line line)
        {
            if(IsLinked(line) == -1)
                return Mathf.Acos(Vector3.Dot(vector, line.vector) / (magnitude * line.magnitude)) * Mathf.Rad2Deg;
            else
                return Mathf.Acos(Vector3.Dot(vector, -line.vector) / (magnitude * line.magnitude)) * Mathf.Rad2Deg;
        }

        public int IsLinked(Line line)
        {
            return pointA.IsEquals(line.pointB) ? 1 : pointB.IsEquals(line.pointA) ? -1 : 0;
        }

        public bool IsEquals(Line line)
        {
            return pointA.IsEquals(line.pointA) && pointB.IsEquals(line.pointB);
        }

        public bool IsReverse(Line line)
        {
            return pointA.IsEquals(line.pointB) && pointB.IsEquals(line.pointA);
        }

        public bool IsEqualsOrReverse(Line line)
        {
            return (pointA.IsEquals(line.pointA) && pointB.IsEquals(line.pointB)) || (pointA.IsEquals(line.pointB) && pointB.IsEquals(line.pointA));
        }

        public bool IsInRange(Vector3 point)
        {
            return point.x > min.x - epsilon && point.x < max.x + epsilon && point.z > min.z -epsilon && point.z < max.z + epsilon;
        }

        public Vector3? IntersectionPoint(Line line)
        {
            return IntersectionPoint(line.a, line.b, line.x);
        }

        public Vector3? IntersectionPoint(float a2, float b2, float? x2)
        {
            if (Mathf.Abs(a - a2) < float.Epsilon)
            {
                return null;
            }
            else if (x != null)
            {
                float y = a2 * x.Value + b2;
                return new Vector3(x.Value, 0, y);
            }
            else if (x2 != null)
            {
                float y = a * x2.Value + b;
                return new Vector3(x2.Value, 0, y);
            }
            else
            {
                //y = a * x + b
                //y = a2 * x + b2
                //a * x + b = a2 * x + b2
                //(a * x) - (a2 * x) = (b2 - b)
                //a - a2 = (b2 - b) / x
                //x = (b2 - b) / (a - a2)

                float xValue = (b2 - b) / (a - a2);
                float y = a * xValue + b;

                return new Vector3(xValue, 0, y);
            }
        }
        public override string ToString()
        {
            return $"PointA: {pointA}, PointB: {pointB}, a: {a}, b: {b}, x: {x}";
        }
    }
}
