using UnityEngine;
using System.Collections;
using UnityEditor;


public class NewSpriteWindow : EditorWindow {

    Texture texture;

    [MenuItem("TWOD/NewSprite")]
    static void OpenWindow()
    {
        NewSpriteWindow window = (NewSpriteWindow)GetWindow(typeof(NewSpriteWindow));
        window.Show();
        // window.minSize = new Vector2(600,300);
        // window.maxSize = new Vector2(800,500);

    }


    void OnEnable()
    {

    }


    void OnGUI()
    {

        texture = (Texture)EditorGUILayout.ObjectField(texture,typeof(Texture),false);



        //Button create
        if (GUILayout.Button("Create"))
        {
            string pathToPrefab = "Prefab/New Sprite";
            GameObject prefab = (GameObject)Resources.Load<GameObject>(pathToPrefab);
            if (prefab != null)
            {
                GameObject obj = GameObject.Instantiate<GameObject>(prefab);
                twodTexture twod = obj.GetComponent<twodTexture>();
                


                this.Close();
            }else
            {
                Debug.LogError("Error Loading prefab on path: 'Resources/Prefab/New Sprite/");
                
            }


        }

    }


}
