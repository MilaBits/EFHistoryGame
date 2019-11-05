using System.Collections;
using System.Collections.Generic;
using SmartwallPackage.GlobalGameSettings;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalGameSettings))]
public class GlobalGameSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GlobalGameSettings ggs = (GlobalGameSettings)target;
        if(GUILayout.Button("Create Settings File"))
        {
            ggs.CreateSettingFile();
        }
    }

}
