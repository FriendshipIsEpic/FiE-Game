using UnityEngine;
using UnityEditor;
using System.Collections;

public class QuickRope2Menu : MonoBehaviour 
{
    [MenuItem("QuickRopes/Ropes/Line Rope")]
    static void AddBasicLineRope()
    {
        QuickRope2 r = QuickRope2.Create(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), BasicRopeTypes.Line);
        r.gameObject.name = "Line_Rope_Start";
        r.ropeEnd.name = "Line_Rope_End";
        Selection.activeGameObject = r.gameObject;
    }

    [MenuItem("QuickRopes/Ropes/Mesh Rope")]
    static void AddBasicMeshRope()
    {
        QuickRope2 r = QuickRope2.Create(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), BasicRopeTypes.Mesh);
        r.gameObject.name = "Mesh_Rope_Start";
        r.ropeEnd.name = "Mesh_Rope_End";
        Selection.activeGameObject = r.gameObject;
    }

    [MenuItem("QuickRopes/Ropes/Prefab Rope")]
    static void AddBasicPrefabRope()
    {
        QuickRope2 r = QuickRope2.Create(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), BasicRopeTypes.Prefab);
        r.gameObject.name = "Prefab_Rope_Start";
        r.ropeEnd.name = "Prefab_Rope_End";
        Selection.activeGameObject = r.gameObject;
    }

    [MenuItem("QuickRopes/Ropes/Cloth Rope")]
    static void AddBasicClothRope()
    {
        QuickRope2 r = QuickRope2.Create(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), BasicRopeTypes.Cloth);
        r.gameObject.name = "Cloth_Rope_Start";
        r.ropeEnd.name = "Cloth_Rope_End";
        Selection.activeGameObject = r.gameObject;
    }

    [MenuItem("QuickRopes/Switch Mesh/Set As Line Type")]
    static void SetLine()
    {
        CleanupObject();
        Selection.activeGameObject.AddComponent<QuickRope2Line>();
    }
    [MenuItem("QuickRopes/Switch Mesh/Set As Line Type", true)]
    static bool ValidateSetLine()
    {
        return ValidateQR2();
    }

    [MenuItem("QuickRopes/Switch Mesh/Set As Mesh Type")]
    static void SetMesh()
    {
        CleanupObject();
        Selection.activeGameObject.AddComponent<QuickRope2Mesh>();
    }
    [MenuItem("QuickRopes/Switch Mesh/Set As Mesh Type", true)]
    static bool ValidateSetMesh()
    {
        return ValidateQR2();
    }

    [MenuItem("QuickRopes/Switch Mesh/Set As Prefab Type")]
    static void SetPrefab()
    {
        CleanupObject();
        Selection.activeGameObject.AddComponent<QuickRope2Prefab>();
    }
    [MenuItem("QuickRopes/Switch Mesh/Set As Prefab Type", true)]
    static bool ValidateSetPrefab()
    {
        return ValidateQR2();
    }

    [MenuItem("QuickRopes/Switch Mesh/Set As Cloth Type")]
    static void SetCloth()
    {
        CleanupObject();
        Selection.activeGameObject.AddComponent<QuickRope2Cloth>();
    }
    [MenuItem("QuickRopes/Switch Mesh/Set As Cloth Type", true)]
    static bool ValidateSetCloth()
    {
        return ValidateQR2();
    }

    [MenuItem("QuickRopes/More ->/Video Tutorials")]
    static void GotoVids()
    {
        Application.OpenURL("http://reverieinteractive.com/unity-assets/quickropes/quickropes-video-tutorials");
    }

    [MenuItem("QuickRopes/More ->/Text Tutorials")]
    static void GotoAPI()
    {
        Application.OpenURL("http://reverieinteractive.com/unity-assets/quickropes/quickropes-scripting-examples");
    }

    static void CleanupObject()
    {
        QuickRope2 r = Selection.activeGameObject.GetComponent<QuickRope2>();

        if (r.GetComponent<QuickRope2Line>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Line>());
        }

        if (r.GetComponent<QuickRope2Mesh>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Mesh>());
        }

        if (r.GetComponent<QuickRope2Cloth>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Cloth>());
        }

        if (r.GetComponent<QuickRope2Prefab>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Prefab>());
        }
    }
    static bool ValidateQR2()
    {
        if (Selection.activeGameObject == null)
            return false;

        return Selection.activeGameObject.GetComponent<QuickRope2>() != null;
    }
}
