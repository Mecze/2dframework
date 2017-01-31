using UnityEngine;
using System.Collections;
using UnityEditor;


public class NewControllerWindow : EditorWindow
{
    [MenuItem("TWOD/Utility Add Controller")]
    static void OpenWindow()
    {
        if (twodController.instance == null || GameObject.FindObjectOfType<twodController>()== null)
        {
            GameObject con = (GameObject)Resources.Load<GameObject>("Prefab\\_twodController");
            if (con == null) { Debug.LogError("Controler Prefab Not Found!"); return; }
            GameObject obj = GameObject.Instantiate(con);
            twodController.instance = obj.GetComponent<twodController>();
            obj.name = "_twodController";
        }else
        {
            Debug.LogWarning("Other twodController Present! there can only be one!");
        }
    }
}
