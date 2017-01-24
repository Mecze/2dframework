/// <summary>
/// 
///     ######################
///     #                    #
///     #        TWOD        #
///     #  Created by Mecze  #
///     #                    #
///     ######################
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using Supyrb;

public delegate void GenericTwodEventHandler();


[CustomEditor(typeof(twodAnimator))]
public class twodAnimatorEditor : Editor {
    private SerializedObject m_Object;
    private SerializedProperty m_Property;
    private ReorderableList list;



    void OnEnable()
    {
        twodAnimator myTwod = (twodAnimator)target;
        m_Object = new UnityEditor.SerializedObject(target);
        SerializedProperty framefrecuency = serializedObject.FindProperty("frameFrecuency");
        //list = new ReorderableList(serializedObject,serializedObject.FindProperty("Animsdf"),)
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("Animations"), false, true, true, true);
        list.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) => {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 80, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x + 80, rect.y, rect.width - 80 - 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("frameFrequency"), GUIContent.none);
        EditorGUI.LabelField(
            new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("animationIndex").intValue.ToString());
        //EditorGUI.La
    };
        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Animations"); };
        list.onAddCallback = (ReorderableList l) => {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = "Animation " + index.ToString(); ;
            element.FindPropertyRelative("frameFrequency").floatValue = framefrecuency.GetValue<float>();
            element.FindPropertyRelative("animationIndex").intValue = index;
            
            };
        list.onChangedCallback = (ReorderableList l) => {
            myTwod.InitializeAnimationsList();
        };

        /*
        list.onAddCallback = (ReorderableList list) => {
            list.list.Add(new twodAnimation("Animation " + list.list.Count.ToString(), list.list.Count, (float)framefrecuency.GetValue<float>()));            
        };
    */
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        twodAnimator myTwod = (twodAnimator)target;
        //SerializedProperty animations = serializedObject.FindProperty("Animations");
        SerializedProperty useDefs = serializedObject.FindProperty("useDefaultFrameFrecuency");
        SerializedProperty framefrecuency = serializedObject.FindProperty("frameFrecuency");
        SerializedProperty startPlaying = serializedObject.FindProperty("startPlaying");
        SerializedProperty animStarts = serializedObject.FindProperty("firstAnimation");
        //SerializedProperty useDefaultFrameFrecuency = serializedObject.FindProperty("useDefaultFrameFrecuency");

        //SerializedProperty texture2d = serializedObject.FindProperty("mainTexture");
        //SerializedProperty useDefault = serializedObject.FindProperty("useDefaultCellSize");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Frame Frecuency", EditorStyles.boldLabel);
        useDefs.SetValue<bool>(EditorGUILayout.Toggle("Use Default Frame Frecuency", useDefs.GetValue<bool>()));
        bool UseDefs = useDefs.GetValue<bool>();
        if (UseDefs)
        {
            //We try to get the instance for the singleton (should be working on editor time)
            twodController tc = twodController.instance;


            if (tc == null)
            {
                //if no instance, we show an error:
                EditorGUILayout.HelpBox("twodController is not present in the Scene!\nNo default values could be found!\nUse Default Frame Frecuency Size will be OFF", MessageType.Error);
                UseDefs = false; //useDef turns to false so when we hit PLAY or another object it will go back to false
            }
            else
            {
                //there IS an instance of the twodController, we get the framefrecuency as an uneditable control
                EditorGUILayout.LabelField("Frame Frecuency: " + tc.frameFrequency.ToString());
            }
        }
        if (!UseDefs)
        {
            //This Draws Frame Frecuency Slider and asigns it
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Frame Frecuency");
            framefrecuency.SetValue<float>(EditorGUILayout.Slider(framefrecuency.GetValue<float>(), 0, 5f));
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);

        serializedObject.Update();
        EditorGUILayout.Space();
        list.DoLayoutList();
        EditorGUILayout.BeginHorizontal();



        if (GUILayout.Button("Update Twod Texture\nPreview", GUILayout.MinHeight(40f), GUILayout.MinWidth(150f)))
        {
            myTwod.InitializeAnimationsList();
        } 
        if (list.serializedProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox("Animation list will be auto filled by default animations with Frame Frecuency from above until it get as much Animations as in the Texture (see Texture Preview)\nExtra Animations entries will be deleted", MessageType.Info);
        }else
        {
            EditorGUILayout.HelpBox("Remenber: if more items than Rows in Texture, they will be deleted", MessageType.Info);
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Start Playing", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Start Animation on Start?", MessageType.None);

        startPlaying.SetValue<bool>(EditorGUILayout.Toggle("Start Playing",startPlaying.GetValue<bool>()));
        if (startPlaying.GetValue<bool>())
        {
            animStarts.SetValue<int>(EditorGUILayout.IntField("Animation to Start From:", animStarts.GetValue<int>()));
        }


        serializedObject.ApplyModifiedProperties();
        







    }


}
