using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Sight)), CanEditMultipleObjects]
public class SightEditor : Editor
{
    private SerializedProperty range; //Reference to the Sight script's range variable

    private SerializedProperty FOV; //Reference to the Sight script's FOV variable

    private SerializedProperty detection; //Reference to the Sight script's detection variable

    void OnEnable()
    {
        //Assign references to Sight's variables
        range = serializedObject.FindProperty("range");
        FOV = serializedObject.FindProperty("fovRadius");
        detection = serializedObject.FindProperty("detection");
    }

    public void OnSceneGUI()
    {
        //Check if the player is detected by the enemy
        if (detection.boolValue == true)
        {
            //Assign a green for if the target is spotted
            Handles.color = new Color(0, 1, 0, 0.1f);
        }
        else
        {
            //Assign a red for if the target is not
            Handles.color = new Color(1, 0, 0, 0.1f);
        }

        //Assign reference to original object transform
        var t = target as Sight;
        var targetTrans = t.transform;

        //Draw an arc representing the enemy's sight line
        DrawArc(targetTrans);
    }

    //Helper func for drawing enemy sightlines
    //Arcs drawn like this to emulate how the vision angle actually works under the hood
    private void DrawArc(Transform targetTrans)
    {
        //Draw one arc going left from the center point
        Handles.DrawSolidArc(targetTrans.position, Vector3.up, targetTrans.forward, FOV.floatValue, range.floatValue);
        //Draw another arc going right from the center pont
        Handles.DrawSolidArc(targetTrans.position, Vector3.up, targetTrans.forward, -FOV.floatValue, range.floatValue);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        //Code for inspector element warnings
        if(range.floatValue <= 0)
        {
            //Check if the range value is above zero, issuing a warning if not
            EditorGUILayout.HelpBox("Range should be greater than 0!", MessageType.Warning);
        }

        if(FOV.floatValue == 0)
        {
            //Check if the FOV is greater than 0
            EditorGUILayout.HelpBox("Enemy FOV is equal to zero! This enemy will struggle to see!", MessageType.Warning);
        }
    }
}
