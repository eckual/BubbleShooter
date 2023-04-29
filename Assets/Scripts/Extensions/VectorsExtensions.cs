using UnityEngine;

namespace Extensions
{
    public static class VectorsExtensions
    {
        public static bool ApproximatelyEqual(this Vector3 pointA, Vector3 pointB, float approximaty = 0.01f, bool compareOnlyY =  false)
        {
            if (compareOnlyY) return Mathf.Abs(pointA.y - pointB.y) < approximaty;
            
            return Mathf.Abs(pointA.x - pointB.x) < approximaty &&
                   Mathf.Abs(pointA.y - pointB.y) < approximaty &&
                   Mathf.Abs(pointA.z - pointB.z) < approximaty;
        }
    }
}
