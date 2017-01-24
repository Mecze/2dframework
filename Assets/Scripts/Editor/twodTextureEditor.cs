using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
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
    private bool textureChanged = false;
    Texture2D maintex;
    private float yPos = 0f;
    private float xPos= 0f;
    private bool firstrun = true;

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
        SerializedProperty AnimSets = serializedObject.FindProperty("AnimationSets");

        //base.OnInspectorGUI();







        //-----------------
        //Texture Part
        //-----------------
        Rect ImageRect = new Rect();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
        EditorGUILayout.Space();
#if UNITY_EDITOR
        textureChanged = false;
        //we check if texture changed since last update:
        if (maintex != texture2d.GetValue<Texture2D>() && !Application.isPlaying && !firstrun)
        {
            textureChanged = true;
            
        }
#endif
        firstrun = false;
        //textureChanged = (maintex != texture2d.GetValue<Texture2D>());
        if (textureChanged) myTwod.SetFrame(null);
        //We get MainTexture and the ObjectField for it
        maintex = texture2d.GetValue<Texture2D>();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture: ", GUILayout.MaxWidth(80f));
        texture2d.SetValue<Texture2D>((Texture2D)EditorGUILayout.ObjectField(maintex, typeof(Texture2D),false));
        EditorGUILayout.EndHorizontal();

        //Cellsize init
        Vector2Int cellSize = cellsize.GetValue<Vector2Int>();
        Rect CellRect;
        int cellX =0;
        int cellY=0;



        //If we HAVE an main texture:
        if (maintex != null)
        {
            //number of cells on X -1 (-1 to match array's)
            cellX = Mathf.RoundToInt(maintex.width / (float)cellSize.x - 1);
            //numer of cells on Y -1 (-1 to match array's)
            cellY = Mathf.RoundToInt(maintex.height / (float)cellSize.y - 1);
            //Draw Main texture preview            
            //Title:
            EditorGUILayout.LabelField("Texture Preview:");

            //We get the Last rect drawn by inspector (mainly for the position)
            ImageRect = GUILayoutUtility.GetLastRect();

            //We feed maintex width and height to our ApplyMaximuns. It will force resize of the image if needed (if too big)
            Vector2 size = ApplyMaximuns(maintex.width, maintex.height, imageclicked);
            //We modify ImageRect with the results of last statement
            ImageRect.x = 40f;
            ImageRect.y = ImageRect.y + ImageRect.height; //y is the position of last Rect drawn, we want the next position
            ImageRect.width = size.x; 
            ImageRect.height = size.y; //we override with our custom size
            //ImageRect is now the exact Rect where the image should be. position and scale

            //if (ImageRect.x > 2f)xPos = ImageRect.x + ImageRect.width;
           // if (ImageRect.y > 2f)yPos = ImageRect.y + ImageRect.height;
            //If clicked it will get big on next frame. imageclicked gets fed into "ApplyMaximuns" next frame
            if (GUILayout.Button(imageclicked ? "-":"+", GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                imageclicked = !imageclicked;
                
            }
            //EditorGUILayout.
            //after we draw an image on top of the button.
            EditorGUI.DrawPreviewTexture(new Rect(ImageRect.x, ImageRect.y, ImageRect.width+ 5f, ImageRect.height+ 5f), maintex);

            //Rect lastRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
            //Vector2 LastRect = EditorGUIUtility.ScreenToGUIPoint(new Vector2(ImageRect.x, ImageRect.y));
            //LastRect = new Vector2(Mathf.Abs(LastRect.x), Mathf.Abs(LastRect.y));
            //We draw X,Y coordinates on the previewTexture
            float cellXOffset = ImageRect.width / ((float)cellX +1);
            float cellYOffset = ImageRect.height / ((float)cellY +1);
            float offset = 2.5f;
            if (imageclicked) offset = 2.5f;
            EditorGUILayout.BeginHorizontal();
            
            for (int x = -1; x <= cellX; x++)
            {
                string str = x.ToString();
                Rect littleRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
                littleRect.y += littleRect.height+5f;
                littleRect.x += ((cellXOffset/2)-offset) + ((cellXOffset+0.5f) * x);
                littleRect.width = cellXOffset;
                littleRect.height = cellYOffset;
                if (x == -1)
                {
                    littleRect.width = 45f;
                    littleRect.x = 0f;
                    str = "Frames: ";
                }


                EditorGUI.LabelField(littleRect,str,EditorStyles.miniLabel);
                //EditorGUILayout.LabelField(x.ToString(), EditorStyles.miniBoldLabel);
                
            }
            EditorGUILayout.EndHorizontal();
            offset = 8f;
            List<twodAnimationSet> animasets = myTwod.AnimationSets;
            for (int y = 0; y <= cellY; y++)
            {
                string str = y.ToString();
                Rect littleRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
                littleRect.y += ((cellYOffset/2)-offset) +(y * (cellYOffset+1f));
                littleRect.x += littleRect.width+10f;
                littleRect.width = cellXOffset+200f;
                littleRect.height = cellYOffset;
                bool setname = false;
                if (animasets != null)
                {
                    if (animasets[y] != null)
                    {
                        str = animasets[y].name;
                        setname = true;
                    }
                }
                if (!setname) str = "Animation " + y.ToString();
                EditorGUI.LabelField(littleRect, str, EditorStyles.miniLabel);

            }

            //Selection marker
            Rect markerRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
            markerRect.x += (currentX * (cellXOffset+1f));// + (cellXOffset / 3);
            markerRect.y += (currentY * (cellYOffset+1f));// + (cellYOffset /3);
            markerRect.width = cellXOffset;
            markerRect.height = cellYOffset;
            //EditorGUI.LabelField(markerRect, "O");
            Color color = Color.grey;
            color.a = 0.5f;
            EditorGUI.DrawRect(markerRect, color);




            GUILayout.Space(ImageRect.height -20f );


            //EditorGUI.DrawPreviewTexture(r, texture2d.GetValue<Texture2D>());       
            //GUILayout.Space(r.y + r.height + 10f + size.y);
        }
        else
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
        
        if (maintex != null && cellSize != null && cellSize.x > 0 && cellSize.y > 0)
        {
            //we do Fail checks -> image wont be usable

            bool failed = false;
            if (maintex.width % cellSize.x != 0 && maintex.width > cellSize.x)
            {
                failed = true;
                EditorGUILayout.HelpBox("Cannot divide the Image\nDividing the width of the Texture results\nin rest. Make sure to feed an image with the correct amount of pixels width and height\nand adjust Cell Size correctly", MessageType.Error);
            }
            //float f = 
            if (maintex.height % cellSize.y != 0 && maintex.height > cellSize.y)
            {
                failed = true;
                EditorGUILayout.HelpBox("Cannot divide the Image\nDividing the height of the Texture results\nin rest. Make sure to feed an image with the correct amount of pixels width and height\nand adjust Cell Size correctly", MessageType.Error);
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

                //note: first spot will be 0,0

                //We now crop our own Texture:


                //If crop failed (failsafe, should fail) we do nothing
                //Sometimes, as editor timing can be cut trough by user actions, it's usefull to have more failsafe statements











                //EditorGUILayout.LabelField("asdf");

                EditorGUILayout.LabelField("Cell Preview:", GUILayout.MaxWidth(100f));
                
                EditorGUILayout.BeginHorizontal();

                    Rect buttonRect = GUILayoutUtility.GetRect(25f, 22f, GUILayout.MaxWidth(25f));
                    buttonRect = GUILayoutUtility.GetRect(25f, 22f, GUILayout.MaxWidth(25f));
                
                    if (GUI.Button(buttonRect, Resources.Load<Texture>("arrow_up")))
                        {
                            currentY = AddWithLimits(currentY, -1, 0, cellY);
                        }

                    Rect cornerRect = GUILayoutUtility.GetRect(25f, 22f, GUILayout.MaxWidth(25f));
                
                EditorGUILayout.EndHorizontal();                    
                EditorGUILayout.BeginHorizontal();

                    buttonRect = GUILayoutUtility.GetRect(25f, 20f, GUILayout.MaxWidth(25f));                
                
                    if (GUI.Button(buttonRect, Resources.Load<Texture>("arrow_left")))
                    {
                        currentX = AddWithLimits(currentX, -1, 0, cellX);
                    }
                    buttonRect = GUILayoutUtility.GetRect(25f, 20f, GUILayout.MaxWidth(25f));
                    buttonRect = GUILayoutUtility.GetRect(25f, 20f, GUILayout.MaxWidth(25f));


                    if (GUI.Button(buttonRect, Resources.Load<Texture>("arrow_right")))
                    {
                        currentX = AddWithLimits(currentX, 1, 0, cellX);
                    }
                
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                    buttonRect = GUILayoutUtility.GetRect(25f, 22f, GUILayout.MaxWidth(25f));
                    buttonRect = GUILayoutUtility.GetRect(25f, 22f, GUILayout.MaxWidth(25f));
                    if (GUI.Button(buttonRect, Resources.Load<Texture>("arrow_down")))
                    {
                        currentY = AddWithLimits(currentY, 1, 0, cellY);
                    }                
                    buttonRect = GUILayoutUtility.GetRect(25f, 22f, GUILayout.MaxWidth(25f));
                EditorGUILayout.EndHorizontal();
                

                //Failsafe. if invaled position, go back to 0,0
                if (!checkValid(currentX,currentY,cellX,cellY)) { currentX = 0; currentY = 0; }

                Texture celltex = myTwod.CropTexture(currentY, currentX);
                if (celltex != null)
                {
                    //This whole part follows the same logic and mechanics than ImageRect setup.
                    //CellRect = GUILayoutUtility.GetLastRect();
                    CellRect = new Rect(cornerRect.x, cornerRect.y, cornerRect.width, cornerRect.height);
                    Vector2 size = ApplyMaximuns(celltex.width, celltex.height, cellclicked);
                    CellRect.x = CellRect.x + CellRect.width + 30f;
                    CellRect.width = size.x;
                    CellRect.height = size.y;



                    if (GUI.Button(new Rect(CellRect.x-25f,CellRect.y,20f,20f), cellclicked?"-":"+"))
                    {
                        cellclicked = !cellclicked;
                        
                    }
                    EditorGUI.DrawPreviewTexture(new Rect(CellRect.x, CellRect.y, CellRect.width + 5f, CellRect.height + 5f), (Texture)celltex);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set this Sprite"))
                    {
                        myTwod.SetFrame(currentY, currentX, true);
                    }
                    EditorGUILayout.HelpBox("Use arrows to chose a Cell from Texture Preview", MessageType.Info);

                }
                else
                {
                    EditorGUILayout.HelpBox("Cropping Failed. Fix other errors and check if your Texture is Readable",MessageType.Error);
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
