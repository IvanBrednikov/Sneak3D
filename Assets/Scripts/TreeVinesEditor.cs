using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeVines))]
public class TreeVinesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TreeVines vines = (TreeVines)target;
        if (GUILayout.Button("Build vines"))
        {
            vines.SpawnVines();
        }
    }
}
