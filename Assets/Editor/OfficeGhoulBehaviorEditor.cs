using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OfficeGhoulBehavior)), CanEditMultipleObjects]
public class OfficeGhoulBehaviorEditor : Editor
{

    private SerializedProperty origin; //origin point for the wandering range
    private SerializedProperty range; //wandering range
    private SerializedProperty showHandles; //Should the sphere be rendered?

    void OnEnable()
    {
        //Assign references to variables
        range = serializedObject.FindProperty("wanderExtents");
        origin = serializedObject.FindProperty("originPoint");
        showHandles = serializedObject.FindProperty("showHandles");
    }

    public void OnSceneGUI()
    {
        //If the dev wants to see the handles
        if (showHandles.boolValue == true)
        {
            //Color the range
            Handles.color = new Color(0, 0, 0, 0.1f);

            //Assign reference to original object transform
            var t = target as OfficeGhoulBehavior;
            var targetTrans = t.transform;

            //Draw an arc representing the enemy's wandering range
            Vector3[] rectangleVerts = new Vector3[4]
            {
                new Vector3(origin.vector3Value.x - range.floatValue/2, origin.vector3Value.y, origin.vector3Value.z - range.floatValue/2),
                new Vector3(origin.vector3Value.x - range.floatValue/2, origin.vector3Value.y, origin.vector3Value.z + range.floatValue/2),
                new Vector3(origin.vector3Value.x + range.floatValue/2, origin.vector3Value.y, origin.vector3Value.z + range.floatValue/2),
                new Vector3(origin.vector3Value.x + range.floatValue/2, origin.vector3Value.y, origin.vector3Value.z - range.floatValue/2),
            };

            //Draw the range the enemy wanders in
            Handles.DrawSolidRectangleWithOutline(rectangleVerts, new Color(1, 1, 1, 1f), new Color(0, 0, 0, 0.1f));
        }
    }
}
