using UnityEngine;
using UnityEditor;
using System.Collections;
using Supyrb;


[CustomEditor(typeof(twodTexture))]
public class twodTextureEditor : Editor
{
    private SerializedObject m_Object;
    private SerializedProperty m_Property;
    private bool imageclicked = false;
    private bool cellclicked = false;
    private int currentX;
    private int currentY;

    void OnEnable()
    {
        m_Object = new UnityEditor.SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        twodTexture myTwod = (twodTexture)target;
        SerializedProperty cellsize = serializedObject.FindProperty("cellSize");
        SerializedProperty texture2d = serializedObject.FindProperty("mainTexture");
        SerializedProperty useDefault = serializedObject.FindProperty("useDefaultCellSize");

        //base.OnInspectorGUI();








        //-----------------
        //Texture Part
        //-----------------
        Rect ImageRect;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        //We getr MainTexture and the ObjectField for it
        Texture2D maintex = texture2d.GetValue<Texture2D>();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture: ", GUILayout.MaxWidth(80f));
        texture2d.SetValue<Texture2D>((Texture2D)EditorGUILayout.ObjectField(maintex, typeof(Texture2D),false));
        EditorGUILayout.EndHorizontal();

        //If we HAVE an main texture:
        if (maintex != null)
        {
            //Draw Main texture preview            
            //Title:
            EditorGUILayout.LabelField("Texture Preview:");

            //We get the Last rect drawn by inspector (mainly for the position)
            ImageRect = GUILayoutUtility.GetLastRect();

            //We feed maintex width and height to our ApplyMaximuns. It will force resize of the image if needed (if too big)
            Vector2 size = ApplyMaximuns(maintex.width, maintex.height, imageclicked);
            //We modify ImageRect with the results of last statement
            ImageRect.y = ImageRect.y + ImageRect.height; //y is the position of last Rect drawn, we want the next position
            ImageRect.width = size.x; 
            ImageRect.height = size.y; //we override with our custom size
            //ImageRect is now the exact Rect where the image should be. position and scale

            //If clicked it will get big on next frame. imageclicked gets fed into "ApplyMaximuns" next frame
            if (GUILayout.Button("", GUILayout.Width(ImageRect.width), GUILayout.Height(ImageRect.height)))
            {
                imageclicked = !imageclicked;
                
            }
            //after we draw an image on top of the button.
            EditorGUI.DrawPreviewTexture(new Rect(ImageRect.x, ImageRect.y, ImageRect.width+ 5f, ImageRect.height+ 5f), maintex);


            //EditorGUI.DrawPreviewTexture(r, texture2d.GetValue<Texture2D>());       
            //GUILayout.Space(r.y + r.height + 10f + size.y);
        }else
        {
            //If we have no Texture, show an error instead!
            EditorGUILayout.HelpBox("No Texture Assigned", MessageType.Error);
        }


        //------------------
        //Cell Size Part
        //------------------
        //we get the bool if we want to use default cellsize value
        bool useDef = useDefault.GetValue<bool>();        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Title:
        EditorGUILayout.LabelField("Cell Size", EditorStyles.boldLabel);

        //We draw the control here:
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Use Default Cell Size:");        
        useDefault.SetValue<bool>(EditorGUILayout.Toggle(useDef));
        EditorGUILayout.EndHorizontal();

        //if it's check (true)        
        if (useDef)
        {
            //We try to get the instance for the singleton (should be working on editor time)
            twodController tc = twodController.instance;

            
            if (tc == null)
            {
                //if no instance, we show an error:
                EditorGUILayout.HelpBox("twodController is not present in the Scene!\nNo default values could be found!\nUse Default Cell Size will be OFF", MessageType.Error);
                useDef = false; //useDef turns to false so when we hit PLAY or another object it will go back to false
            }else
            {
                //there IS an instance of the twodController, we get the cellsize as an uneditable control
                EditorGUILayout.LabelField("Cell Size: X: " + tc.cellSize.x.ToString() + " Y: " + tc.cellSize.y.ToString());
            }
        }
        //If it isnt check (false)
        if (!useDef)
        {
            //This line will draw and set a vector2int as if it were a Vector2 (float)
            cellsize.SetValue<Vector2Int>(Vector2Int.RoundToVector2Int(EditorGUILayout.Vector2Field("Cell Size", cellsize.GetValue<Vector2Int>().toVector2())));
            //note: Vector2Int is a class created for twod which hold 2 integers
        }

        //Cell Preview
        //If we have no main texture, we dont draw anything
        Vector2Int cellSize = cellsize.GetValue<Vector2Int>();
        Rect CellRect;
        if (maintex != null && cellSize != null && cellSize.x > 0 && cellSize.y > 0)
        {
            //we do Fail checks -> image wont be usable

            bool failed = false;
            if (maintex.width % cellSize.x != 0 && maintex.width > cellSize.x)
            {
                failed = true;
                EditorGUILayout.HelpBox("Cannot divide the Image\nDividing the width of the Texture results\nin rest. Make sure to feed the correct amount of pixels width and height", MessageType.Error);
            }
            //float f = 
            if (maintex.height % cellSize.y != 0 && maintex.height > cellSize.y)
            {
                failed = true;
                EditorGUILayout.HelpBox("Cannot divide the Image\nDividing the height of the Texture results\nin rest. Make sure to feed the correct amount of pixels width and height", MessageType.Error);
            }
            if (maintex.width < cellSize.x)
            {
                failed = true;
                EditorGUILayout.HelpBox("Cell Size (X) is too big\nIt's bigger than the image!", MessageType.Error);
            }
            if (maintex.height < cellSize.y)
            {
                failed = true;
                EditorGUILayout.HelpBox("Cell Size (Y) is too big\nIt's bigger than the image!", MessageType.Error);
            }

            //if we didnt fail:
            if (!failed)
            {
                //number of cells on X -1 (-1 to match array's)
                int cellX = Mathf.RoundToInt(maintex.width / (float)cellSize.x - 1);
                //numer of cells on Y -1 (-1 to match array's)
                int cellY = Mathf.RoundToInt(maintex.height / (float)cellSize.y - 1);
                //note: first spot will be 0,0

                //We now crop our own Texture:
                

                //If crop failed (failsafe, should fail) we do nothing
                //Sometimes, as editor timing can be cut trough by user actions, it's usefull to have more failsafe statements
                
                EditorGUILayout.BeginHorizontal();

                //EditorGUILayout.LabelField("asdf");
                EditorGUILayout.LabelField("Cell Preview:", GUILayout.MaxWidth(100f));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("<"))
                {
                    currentX = AddWithLimits(currentX, -1, 0, cellX);
                }
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("^"))
                {
                    currentY = AddWithLimits(currentY, -1, 0, cellY);
                }
                if (GUILayout.Button("v"))
                {
                    currentY = AddWithLimits(currentY, 1, 0, cellY);
                }
                EditorGUILayout.EndVertical();
                if (GUILayout.Button(">"))
                {
                    currentX = AddWithLimits(currentX, 1, 0, cellX);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
                
                //Failsafe. if invaled position, go back to 0,0
                if (!checkValid(currentX,currentY,cellX,cellY)) { currentX = 0; currentY = 0; }

                Texture celltex = myTwod.CropTexture(currentY, currentX);
                if (celltex != null)
                {
                    //This whole part follows the same logic and mechanics than ImageRect setup.
                    CellRect = GUILayoutUtility.GetLastRect();
                    Vector2 size = ApplyMaximuns(celltex.width, celltex.height, cellclicked);
                    CellRect.y = CellRect.y + CellRect.height;
                    CellRect.width = size.x;
                    CellRect.height = size.y;



                    if (GUILayout.Button("", GUILayout.Width(size.x), GUILayout.Height(size.y)))
                    {
                        cellclicked = !cellclicked;
                        //Debug.Log(cellclicked.ToString());
                    }
                    EditorGUI.DrawPreviewTexture(new Rect(CellRect.x, CellRect.y, CellRect.width + 5f, CellRect.height + 5f), (Texture)celltex);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set this Sprite"))
                    {
                        myTwod.SetFrame(currentY, currentX, true);
                    }


                }
            }
            else
            {
                //EditorGUILayout.HelpBox("No Texture Assigned", MessageType.Error);
            }
        }
        //Failed = true
        else
        {
            //do nothing
        }




        EditorGUILayout.Space();
        EditorGUILayout.Space();
        m_Object.ApplyModifiedProperties();





    }



    Vector2 ApplyMaximuns(float x, float y, bool big)
    {
        float MaxX;
        float MaxY;
        Vector2 r;
        if (!big)
        {
            MaxX = 60f;
            MaxY = 80f;

        }else
        {
            MaxX = 350f;
            MaxY = 500f;
        }

        while (x > MaxX || y > MaxY)
        {
            if (x > MaxX)
            {
                y = (MaxX * y) / x;
                x = MaxX;
            }
            if (y > MaxY)
            {
                x = (MaxY * x) / y;
                y = MaxY;
            }
        }
        r = new Vector2(x, y);

        



        return r;
    }

    int AddWithLimits(int a, int b, int limitInf,int limitSup)
    {
        a = a + b;

        while (a > limitSup || a < limitInf)
        {

            //if a is to big, we loop arround
            if (a > limitSup)
            {
                //we go back to inferior limit plus how much we overrun superior limit
                a = limitInf + (a - limitSup) -1;
            }
            //same for inferior limit:
            if (a < limitInf)
            {
                a = limitSup - (Mathf.Abs(a - limitInf)) +1;
            }
        }
        return a;

    }


    bool checkValid(int x, int y, int MaxX, int Maxy)
    {
        bool r = true;
        if (x < 0 || x > MaxX || y < 0 || y > Maxy) r = false;
        return r;
    }

}
