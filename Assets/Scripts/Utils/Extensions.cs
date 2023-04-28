using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static bool ApproximatelyEqual(this Vector3 a, Vector3 b, float approximaty = 0.01f)
    {
        return Mathf.Abs(a.x - b.x) < approximaty &&
               Mathf.Abs(a.y - b.y) < approximaty &&
               Mathf.Abs(a.z - b.z) < approximaty;
    }
}
