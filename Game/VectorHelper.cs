using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class VectorHelper
{
    public static bool IsEquals(this Vector3 vector, Vector3 vector2, float epsilon)
    {
        return Math.Abs(vector.x - vector2.x) < epsilon && Math.Abs(vector.y - vector2.y) < epsilon && Math.Abs(vector.z - vector2.z) < epsilon;
    }

    public static void test()
    {
        UnityEngine.Vector3 vector = new UnityEngine.Vector3();
        vector.IsEquals();
    }
}

