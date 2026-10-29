using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hearing)), CanEditMultipleObjects]
public class HearingEditor : Editor
{
    private SerializedProperty loud;
    private SerializedProperty quiet;

    private void OnEnable()
    {
        loud = serializedObject.FindProperty("loudRadius");
        quiet = serializedObject.FindProperty("quietRadius");
    }


    public void OnSceneGUI()
    {
        //Assign reference to original object transform
        var t = target as Hearing;
        var targetTrans = t.transform;

        Handles.color = new Color(0, 0, 0, 0.1f); //Set color for loud radius

        //Draw a circle representing the radius of the loud sound hearing range
        DrawRange(targetTrans, loud.floatValue);

        Handles.color = new Color(0, 0, 0, 0.1f); //Set color for quiet radius


        //Draw a circle representing the radius of the quiet sound hearing range
        DrawRange(targetTrans, quiet.floatValue);

    }

    //Helper function for drawing a circle representing a hearing radius
    private void DrawRange(Transform origin, float range)
    {
        //Range of hearing uses sqrt to accurately represent the range of hearing based on the checks
        Handles.DrawSolidDisc(origin.position, Vector3.up, Mathf.Sqrt(range));
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        //Code for inspector element warnings
        if (loud.floatValue <= 0)
        {
            //If loud sound range is less than 0 put up a warning
            EditorGUILayout.HelpBox("Enemy cannot hear loud sounds!", MessageType.Warning);
        }

        if (quiet.floatValue == 0)
        {
            //If quiet sound range is less than 0 put up a warning
            EditorGUILayout.HelpBox("Enemy cannot hear quiet sounds!", MessageType.Warning);
        }
    }
}
