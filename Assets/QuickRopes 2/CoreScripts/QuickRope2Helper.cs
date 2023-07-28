using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuickRope2Helper
{
    public static bool HasMoved(ref Vector3 prevPoint, Vector3 curPoint)
    {
        bool r = Vector3.Distance(curPoint, prevPoint) >= 0.01f;

        if (r) prevPoint = curPoint;

        return r;
    }
}

[System.Serializable]
public class QuickRopeSpline
{
    public List<Vector3> Points;

    public Vector3 Interpolate(float t)
    {
        if (Points.Count < 4)
            return Vector3.zero;

        int numSections = Points.Count - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = Points[currPt];
        Vector3 b = Points[currPt + 1];
        Vector3 c = Points[currPt + 2];
        Vector3 d = Points[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    public void DrawGizmo(float t, int precision, Color splineColor)
    {
        if (Points.Count < 3)
            return;

        Gizmos.color = splineColor;
        Vector3 prevPt = Interpolate(0);

        for (int i = 1; i <= 100; i++)
        {
            float pm = (float)i / (float)100;
            Vector3 currPt = Interpolate(pm);
            Gizmos.DrawLine(currPt, prevPt);
            prevPt = currPt;
        }

        Gizmos.color = Color.white;
    }
}