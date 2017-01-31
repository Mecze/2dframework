using UnityEngine;
using System.Collections;
using UnityEditor;


public class NewSpriteWindow : EditorWindow
{

    Texture texture;
    //Texture lastTexture;
    FilterMode filterMode;
    Vector2Int cellSize;
    bool useDefaultCellSize;
    bool useDefCSERROR;
    bool useDefaultFilterMode;
    bool useDefFMERROR;

    [MenuItem("TWOD/New twodSprite")]
    [MenuItem("GameObject/2D Object/New twodSprite")]    
    static void OpenWindow()
    {
        NewSpriteWindow window = (NewSpriteWindow)GetWindow(typeof(NewSpriteWindow));
        window.minSize = new Vector2(330, 250);
        window.maxSize = new Vector2(340, 320);        
        GUIContent asdf = new GUIContent();
        asdf.text = "New twodSprite";
        window.titleContent = asdf;
        window.Show();
        
        

    }


    void OnEnable()
    {
        useDefCSERROR = false;
        useDefFMERROR = false;
        cellSize = new Vector2Int(0, 0);
    }


    void OnGUI()
    {
        

        //If textureChanged during this frame (commented now, maybe will use on the future)
        //bool textureChanged = false;

        //Label for Texture
        EditorGUILayout.LabelField("Texture:", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        //Texture Part
        texture = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), false,GUILayout.MaxWidth(330f));
        //if (texture != lastTexture) textureChanged = true;
        //lastTexture = texture;

        //No Texture? we show only a Helpbox
        if (texture == null)
        {
            EditorGUILayout.HelpBox("Insert your Texture Here.\nTexture, Texture2D and Sprites supported", MessageType.Info);
        }
        else
        {
            //FILTER MODE:
            //DEFAULT FILTER MODE:
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Filter Mode:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use Default Filter Mode:", GUILayout.MaxWidth(160f));
            useDefaultFilterMode = EditorGUILayout.Toggle(useDefaultFilterMode);
            EditorGUILayout.EndHorizontal();
            //VALUE:
            if (useDefaultFilterMode)
            {
                //error management
                if (twodController.instance == null)
                {
                    useDefFMERROR = true;
                    useDefaultFilterMode = false;
                }
                else
                {
                    EditorGUILayout.LabelField("Filter Mode: " + twodController.instance.filterMode.ToString());
                }
            }
            else
            {
                if (useDefFMERROR)
                {
                    EditorGUILayout.HelpBox("No twodController in the Scene found! Please add one and configure it or avoid using default values.\nFor now, this setting will be OFF", MessageType.Error);
                    Rect r = GUILayoutUtility.GetLastRect();
                    r.x = r.x + r.width - 25f;
                    r.y = r.y + 5f;
                    r.height = 20f;
                    r.width = 20f;
                    if (GUI.Button(r, "x"))
                    {
                        useDefFMERROR = false;
                    }

                }
                //Value insertion
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Filter Mode: ", GUILayout.MaxWidth(160f));
                filterMode = (FilterMode)EditorGUILayout.EnumPopup(filterMode, GUILayout.MaxWidth(160f));
                EditorGUILayout.EndHorizontal();
            }
                //-------------------------------

                //CELLSIZE
                //DEFAULT CELLSIZE:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Cell Size:", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Use Default Cell Size:", GUILayout.MaxWidth(160f));
                useDefaultCellSize = EditorGUILayout.Toggle(useDefaultCellSize);
                EditorGUILayout.EndHorizontal();
                //VALUE:
                if (useDefaultCellSize)
                {
                    //error management
                    if (twodController.instance == null)
                    {
                        useDefCSERROR = true;
                        useDefaultCellSize = false;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Cell Size: " + twodController.instance.cellSize.ToString());
                    }
                }
                else
                {
                    if (useDefCSERROR)
                    {
                        EditorGUILayout.HelpBox("No twodController in the Scene found! Please add one and configure it or avoid using default values.\nFor now, this setting will be OFF", MessageType.Error);
                        Rect r = GUILayoutUtility.GetLastRect();
                        r.x = r.x + r.width - 25f;
                        r.y = r.y + 5f;
                        r.height = 20f;
                        r.width = 20f;
                        if (GUI.Button(r, "x"))
                        {
                            useDefCSERROR = false;
                        }

                    }
                    //Value insertion
                    //EditorGUILayout.BeginHorizontal();
                    //EditorGUILayout.LabelField("Cell Size: ", GUILayout.MaxWidth(160f));
                    //if (cellSize != null)
                    cellSize = Vector2Int.RoundToVector2Int(EditorGUILayout.Vector2Field("Cell Size: ", cellSize.toVector2(),GUILayout.MaxWidth(200f)));
                    //EditorGUILayout.EndHorizontal();



                }


        EditorGUILayout.Space();
        //Button create
        if (GUILayout.Button("Create new twodSprite",GUILayout.Width(200f)))
            {
                string pathToPrefab = "Prefab/twodSprite";
                GameObject prefab = (GameObject)Resources.Load<GameObject>(pathToPrefab);
                if (prefab != null)
                {
                    GameObject obj = GameObject.Instantiate<GameObject>(prefab);
                    twodTexture twod = obj.GetComponent<twodTexture>();
                    bool success = twod.SetThisSprite(texture, filterMode, cellSize, useDefaultCellSize, useDefaultFilterMode);
                    if (success)
                    {
                        twod.SetFrame(0, 0, true);
                        obj.GetComponent<twodAnimator>().InitializeAnimationsList();                        
                        string candidate = "new twodSprite";
                        int x = 0;
                        string xs = "";
                        while (GameObject.Find(candidate+xs)!= null)
                        {
                            x++;
                            xs = x.ToString();
                        }
                        obj.name = candidate + xs;


                        this.Close();
                    }else
                    {
                        GameObject.Destroy(obj);
                        EditorGUILayout.HelpBox("Something went wrong!", MessageType.Error);
                    }
                }
                else
                {
                    Debug.LogError("Error Loading prefab on path: 'Resources/Prefab/twodSprite");

                }


            }
        }
    }


}

