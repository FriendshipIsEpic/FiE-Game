
using UnityEngine;
using System.Collections;


public class RopeTubeRenderer
{
    public bool calculateTangents = false; // turn off to boost performance, results in odd reflections

    GameObject _gameObject;
    public GameObject gameObject { get { return _gameObject; } }

    Transform _transform;
    public Transform transform { get { return _transform; } }

    int targetVertexCount; // points.Length * ( edgeCount+1 ) one extra hidden edge for UV wrapping

    // user manipulated variables //
    Vector3[] points = new Vector3[0];
    float radius = 0.5f;
    float[] radiuses;
    int edgeCount = 12; // minimum is three
    Color[] pointColors;
    Rect capUVRect = new Rect(0, 0, 1, 1);
    Rect bodyUVRect = new Rect(0, 0, 1, 1);

    // internal variables //
    Vector3[] vertices = new Vector3[0];
    Vector3[] normals = new Vector3[0];
    int[] triangles;
    Vector2[] uvs;
    Vector4[] tangents = new Vector4[0];
    Color[] colors;
    Vector3[] circleLookup;
    Quaternion[] rotations = new Quaternion[0];

    MeshFilter filter;

    bool updateCircleLookupFlag;
    bool updateUVsFlag;
    bool updateTrianglesFlag;
    bool updateTangentsFlag;
    bool updateColorsFlag;
    bool redrawFlag;

    bool pointCountChanged;

    const float TWO_PI = Mathf.PI * 2;

    Vector3 pastUp; // used for calculating the rotation
    public Vector3 up
    {
        get { return pastUp != Vector3.zero ? pastUp : Vector3.up; }
        set { pastUp = value; }

    }

    private Mesh _mesh;
    public Mesh mesh
    {
        get { return _mesh; }
    }


    public RopeTubeRenderer(GameObject _gameObject, bool useMeshOnly)
    {
        if (!useMeshOnly)
        {
            this._gameObject = _gameObject;
            this._transform = _gameObject.transform;

            // ensure necessary components //
            MeshFilter filter = _gameObject.GetComponent<MeshFilter>();
            if (filter == null) filter = _gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = _gameObject.GetComponent<MeshRenderer>();
            if (renderer == null) renderer = _gameObject.AddComponent<MeshRenderer>();

            _mesh = new Mesh();
            _mesh.name = "RopeTube_" + _gameObject.GetInstanceID();
            filter.mesh = _mesh;

            if (renderer.sharedMaterial == null)
                renderer.sharedMaterial = (Material)Resources.Load("Materials/Rope", typeof(Material));
        }
        else
        {
            this._gameObject = _gameObject;
            this._transform = _gameObject.transform;

            _mesh = new Mesh();
            _mesh.name = "RopeTube_" + _gameObject.GetInstanceID();
        }
    }


    public void Update()
    {
        if (points.Length == 0) return;

        if (updateCircleLookupFlag) { UpdateCircleLookup(); }
        if (updateUVsFlag) { UpdateUVs(); }
        if (updateTangentsFlag && calculateTangents) { UpdateTangents(); }
        if (updateTrianglesFlag) { UpdateTriangles(); }
        if (redrawFlag) { ReDraw(); }
        if (updateColorsFlag) { UpdateColors(); }

        updateCircleLookupFlag = false;
        updateTangentsFlag = false;
        updateTrianglesFlag = false;
        updateUVsFlag = false;
        redrawFlag = false;
        updateColorsFlag = false;
    }


    void ReDraw()
    {
        // update array length //
        if (vertices.Length != targetVertexCount)
        {
            _mesh.triangles = new int[0]; // avoid "Mesh.vertices is too small" error message
            vertices = new Vector3[targetVertexCount];
        }
        if (normals.Length != targetVertexCount) normals = new Vector3[targetVertexCount];

        int v = 1 + edgeCount + 1; // start beyond the start cap

        Vector3 minBounds = new Vector3(10000, 10000, 10000);
        Vector3 maxBounds = new Vector3(-10000, -10000, -10000);
        for (int p = 0; p < points.Length; p++)
        {
            //rotations[p] *= Quaternion.Euler(0, 90, 0);

            if (radiuses != null)
            {
                // check min an max bounds //
                if (points[p].x - radiuses[p] < minBounds.x) minBounds.x = points[p].x - radiuses[p];
                if (points[p].y - radiuses[p] < minBounds.y) minBounds.y = points[p].y - radiuses[p];
                if (points[p].z - radiuses[p] < minBounds.z) minBounds.z = points[p].z - radiuses[p];
                if (points[p].x + radiuses[p] > maxBounds.x) maxBounds.x = points[p].x + radiuses[p];
                if (points[p].y + radiuses[p] > maxBounds.y) maxBounds.y = points[p].y + radiuses[p];
                if (points[p].z + radiuses[p] > maxBounds.z) maxBounds.z = points[p].z + radiuses[p];

                // set vertices and normals //
                for (int s = 0; s < edgeCount + 1; s++)
                {
                    vertices[v] = _transform.InverseTransformPoint(points[p] + rotations[p] * circleLookup[s] * radiuses[p]);
                    normals[v] = _transform.InverseTransformDirection(rotations[p] * circleLookup[s]);
                    v++;
                }

            }
            else
            {
                // check min an max bounds //
                if (points[p].x - radius < minBounds.x) minBounds.x = points[p].x - radius;
                if (points[p].y - radius < minBounds.y) minBounds.y = points[p].y - radius;
                if (points[p].z - radius < minBounds.z) minBounds.z = points[p].z - radius;
                if (points[p].x + radius > maxBounds.x) maxBounds.x = points[p].x + radius;
                if (points[p].y + radius > maxBounds.y) maxBounds.y = points[p].y + radius;
                if (points[p].z + radius > maxBounds.z) maxBounds.z = points[p].z + radius;

                // set vertices and normals //
                for (int s = 0; s < edgeCount + 1; s++)
                {
                    vertices[v] = _transform.InverseTransformPoint(points[p] + rotations[p] * circleLookup[s] * radius);
                    normals[v] = _transform.InverseTransformDirection(rotations[p] * circleLookup[s]);
                    v++;
                }
            }
        }

        // start and end caps //
        vertices[0] = _transform.InverseTransformPoint(points[0]);
        vertices[vertices.Length - 1] = _transform.InverseTransformPoint(points[points.Length - 1]);
        normals[0] = _transform.InverseTransformDirection(rotations[0] * Vector3.forward);
        normals[targetVertexCount - 1] = _transform.InverseTransformDirection(rotations[0] * -Vector3.forward);
        v = 1;
        for (int s = 0; s < edgeCount + 1; s++)
        {
            vertices[v] = vertices[v + edgeCount + 1];
            normals[v] = normals[0];
            v++;
        }
        v = vertices.Length - edgeCount - 2;
        for (int s = 0; s < edgeCount + 1; s++)
        {
            vertices[v] = vertices[v - edgeCount - 1];
            normals[v] = normals[targetVertexCount - 1];
            v++;
        }

        // update mesh //
        _mesh.vertices = vertices;
        if (updateUVsFlag) _mesh.uv = uvs;
        if (updateTrianglesFlag) _mesh.triangles = triangles;
        _mesh.normals = normals;
        if (calculateTangents) _mesh.tangents = tangents;

        /*
        // update bounds //
        Vector3 boundsSize = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, maxBounds.z - minBounds.z);
        Vector3 boundsCenter = new Vector3(minBounds.x + boundsSize.x * 0.5f, minBounds.y + boundsSize.y * 0.5f, minBounds.z + boundsSize.z * 0.5f);
        _mesh.bounds = new Bounds(boundsCenter, boundsSize);
    
        */
        _mesh.RecalculateBounds();
    }


    void UpdateCircleLookup()
    {
        circleLookup = new Vector3[edgeCount + 1]; // add one more hidden side for UV wrapping
        float interpolatorMult = 1 / (float)edgeCount;
        for (int s = 0; s < circleLookup.Length; s++)
        {
            float interpolator = s * interpolatorMult * TWO_PI;
            circleLookup[s] = new Vector3(0, Mathf.Cos(interpolator), Mathf.Sin(interpolator));
        }
    }


    void UpdateUVs()
    {
        uvs = new Vector2[targetVertexCount];
        float interpolatorUmult = 1 / (points.Length - 1f);
        float interpolatorVmult = 1 / (float)edgeCount;
        int uv = 0;
        // start cap //
        uvs[uv++] = new Vector2(capUVRect.width * 0.5f + capUVRect.x, capUVRect.height * 0.5f + capUVRect.y);
        for (int v = 0; v < edgeCount + 1; v++)
        {
            float angle = v * interpolatorVmult * TWO_PI + Mathf.PI * 0.5f;
            uvs[uv++] = new Vector2(uvs[0].x + Mathf.Cos(angle) * 0.5f * capUVRect.width, uvs[0].y + Mathf.Sin(angle) * 0.5f * capUVRect.height);
        }
        // body //
        for (int u = 0; u < points.Length; u++)
        {
            float interpolatorU = u * interpolatorUmult;
            for (int v = 0; v < edgeCount + 1; v++)
            {
                float interpolatorV = v * interpolatorVmult;
                uvs[uv++] = new Vector2(bodyUVRect.x + interpolatorU * bodyUVRect.width, bodyUVRect.y + interpolatorV * bodyUVRect.height);
            }
        }
        // end cap //
        for (int v = 0; v < edgeCount + 1; v++) uvs[uv++] = uvs[v + 1];
        uvs[uv++] = uvs[0];
    }


    void UpdateTangents()
    {
        tangents = new Vector4[targetVertexCount];
        int t = 0;
        // start cap //
        Vector3 tangent = _transform.InverseTransformDirection(rotations[0] * Vector3.right);
        for (int s = 0; s < edgeCount + 2; s++)
        {
            tangents[t++] = new Vector4(tangent.x, tangent.y, tangent.z, 1);
        }
        // body //
        for (int r = 0; r < rotations.Length; r++)
        {
            tangent = _transform.InverseTransformDirection(rotations[r] * Vector3.forward);
            if (calculateTangents)
            {
                for (int s = 0; s < edgeCount + 1; s++)
                {
                    tangents[t++] = new Vector4(tangent.x, tangent.y, tangent.z, 1);
                }
            }
        }
        // end cap //
        tangent = _transform.InverseTransformDirection(rotations[rotations.Length - 1] * Vector3.left);
        for (int s = 0; s < edgeCount + 2; s++)
        {
            tangents[t++] = new Vector4(tangent.x, tangent.y, tangent.z, 1);
        }
    }


    void UpdateTriangles()
    {
        int bodyTriangleCount = (points.Length - 1) * edgeCount * 2;
        int capsTriangleCount = 2 * edgeCount;
        triangles = new int[(bodyTriangleCount + capsTriangleCount) * 3];
        int v = 1; int t = 0;
        // begin cap //
        for (int e = 0; e < edgeCount; e++)
        {
            triangles[t++] = v + 1;
            triangles[t++] = v;
            triangles[t++] = 0;
            v++;
        }
        v++; // skip hidden vertex

        // body //
        int[] quad = new int[] { 0, 1, edgeCount + 2, 0, edgeCount + 2, edgeCount + 1 };
        for (int p = 0; p < points.Length - 1; p++)
        {
            for (int e = 0; e < edgeCount; e++)
            {
                for (int q = 0; q < quad.Length; q++) triangles[t++] = v + quad[q];
                v++;
            }
            v++; // skip hidden vertex
        }

        v++; // skip hidden vertex
        v += edgeCount; // move to next band

        // end cap //
        for (int e = 0; e < edgeCount; e++)
        {

            triangles[t++] = v;
            triangles[t++] = v + 1;
            triangles[t++] = targetVertexCount - 1;
            v++;
        }
    }


    void UpdateColors()
    {
        if (pointColors != null)
        {
            colors = new Color[targetVertexCount];
            // cap start //
            int v = 0;
            colors[v++] = pointColors[0];
            for (int s = 0; s < edgeCount + 1; s++) colors[v++] = pointColors[0];

            // body //
            for (int p = 0; p < points.Length; p++)
            {
                for (int s = 0; s < edgeCount + 1; s++)
                {
                    colors[v++] = pointColors[p];
                }
            }
            // cap end //
            for (int s = 0; s < edgeCount + 1; s++)
            {
                colors[v++] = pointColors[pointColors.Length - 1];
            }
            colors[v++] = pointColors[pointColors.Length - 1];

            mesh.colors = colors;
        }
    }


    ////////////
    // PUBLIC //
    ////////////


    public void SetPointCount(int pointCount)
    {
        if (pointCount < 2)
        {
            Debug.LogWarning("TubeRenderer must have at two three points.");
            return;
        }

        updateTrianglesFlag = true;
        updateUVsFlag = true;
        updateColorsFlag = true;
        updateTangentsFlag = true;
        if (circleLookup == null) updateCircleLookupFlag = true;
        redrawFlag = true;

        targetVertexCount = pointCount * (edgeCount + 1);

        this.points = new Vector3[pointCount];
    }


    public void SetPointsAndRotations(Vector3[] points, Quaternion[] rotations)
    {
        if (points.Length < 2)
        {
            Debug.LogWarning("RopeTubeRenderer must have at two three points.");
            return;
        }

        if (points.Length != rotations.Length)
        {
            Debug.LogWarning("point array must match length of rotation array.");
            return;
        }

        int lastPointCount = (this.points != null) ? this.points.Length : 0;
        if (points.Length != lastPointCount)
        {
            updateTrianglesFlag = true;
            updateUVsFlag = true;
            updateColorsFlag = true;
        }
        updateTangentsFlag = true;
        if (circleLookup == null) updateCircleLookupFlag = true;
        redrawFlag = true;

        if (radiuses != null) if (points.Length != radiuses.Length) radiuses = null;

        targetVertexCount = (points.Length + 2) * (edgeCount + 1) + 2; // two cap end points

        this.points = points;
        this.rotations = rotations;
    }


    public void SetEdgeCount(int edgeCount)
    {
        if (edgeCount < 3) edgeCount = 3;

        this.edgeCount = edgeCount;

        updateTrianglesFlag = true;
        updateUVsFlag = true;
        updateCircleLookupFlag = true;
        updateColorsFlag = true;
        redrawFlag = true;

        targetVertexCount = (points.Length + 2) * (edgeCount + 1) + 2; // two end points
    }


    public void SetRadius(float radius)
    {
        this.radius = radius;

        redrawFlag = true;
    }


    public void SetRadiuses(float[] radiuses)
    {
        if (radiuses == null)
        {
            this.radiuses = null;
            return;
        }
        if (radiuses.Length != points.Length)
        {
            Debug.Log(
                "TubeRenderer only receives as many radius values as it has points. " +
                "Use SetPoints() or SetPointCount() before using SetRadiuses()"
            );
            return;
        }

        this.radiuses = radiuses;

        redrawFlag = true;
    }


    public void SetColors(Color[] colors)
    {
        if (colors.Length != points.Length)
        {
            Debug.Log(
                "TubeRenderer only receives as many color values as it has points. " +
                "Use SetPoints() or SetPointCount() before using SetColors()"
            );
            return;
        }
        pointColors = colors;

        updateColorsFlag = true;
    }


    public void SetBodyUVRect(Rect uvRect)
    {
        bodyUVRect = uvRect;

        updateUVsFlag = true;
    }


    public void SetCapsUVRect(Rect uvRect)
    {
        capUVRect = uvRect;

        updateUVsFlag = true;
    }


    // GETTERS //


    public Vector3[] Points()
    {
        Vector3[] copy = new Vector3[points.Length];
        points.CopyTo(copy, 0);
        return copy;
    }


    public float[] Radiuses()
    {
        float[] copy = new float[radiuses.Length];
        radiuses.CopyTo(copy, 0);
        return copy;
    }


    public int EdgeCount() { return edgeCount; }

}