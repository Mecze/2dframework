/// <summary>
/// 
///     ######################
///     #                    #
///     #        TWOD        #
///     #  Created by Mecze  #
///     #       (2017)       #
///     #                    #
///     ######################
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Supyrb;

[CustomEditor(typeof(twodController))]
public class twodControllerEditor : Editor
{
    private SerializedObject m_Object;
    

    void OnEnable()
    {
        twodController tc = (twodController)target;
        if (twodController.instance == null) twodController.instance = tc; //failsafe
        SerializedProperty cellsize = serializedObject.FindProperty("cellSize");



    }
    public override void OnInspectorGUI()
    {
        SerializedProperty cellsize = serializedObject.FindProperty("cellSize");
        SerializedProperty frameFrecuency = serializedObject.FindProperty("frameFrequency");
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Default Values:", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        cellsize.SetValue<Vector2Int>(Vector2Int.RoundToVector2Int(EditorGUILayout.Vector2Field("Default Cell Size", cellsize.GetValue<Vector2Int>().toVector2())));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Default Frame Frecuency");
        frameFrecuency.SetValue<float>(EditorGUILayout.Slider(frameFrecuency.GetValue<float>(),0,5f));


    }
}
