using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OfficeGhoulBehavior)), CanEditMultipleObjects]
public class OfficeGhoulBehaviorEditor : Editor
{

    private SerializedProperty origin;
    private SerializedProperty range;

    void OnEnable()
    {
        //Assign references to Sight's variables
        range = serializedObject.FindProperty("wanderExtents");
        origin = serializedObject.FindProperty("originPoint");
    }

    public void OnSceneGUI()
    {
        //Color the range
        Handles.color = new Color(0, 0, 0, 0.1f);

        //Assign reference to original object transform
        var t = target as OfficeGhoulBehavior;
        var targetTrans = t.transform;

        //Draw an arc representing the enemy's sight line
        Handles.DrawSolidDisc(origin.vector3Value, Vector3.up, range.floatValue);
    }
}
