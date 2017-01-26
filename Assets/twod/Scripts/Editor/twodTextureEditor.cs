using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Supyrb;
using System;

[CustomEditor(typeof(twodTexture))]
public class twodTextureEditor : Editor
{
    //Texture2D chosenTexture = null;
    bool useDef = false;
    private SerializedObject m_Object;
    private SerializedProperty m_Property;
    private bool imageclicked = false;
    private bool cellclicked = false;
    private int currentX;
    private int currentY;
    private bool textureChanged = false;
    Texture2D maintex;
    Texture2D chosentex;
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
        SerializedProperty chosenTexture = serializedObject.FindProperty("ChosedTexture");
        SerializedProperty useDefault = serializedObject.FindProperty("useDefaultCellSize");
        SerializedProperty AnimSets = serializedObject.FindProperty("AnimationSets");




        //-----------------
        //Texture Part
        //-----------------
        #region Texture Part

        //note: this part bear a lot of checks that will be used later on by other parts
        //of this editor script

        //We initialize ImageRect, used later
        Rect ImageRect = new Rect();
        //We draw the tittle
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        //we check if texture was changed last check.
        //it checks for (chosentex)
#if UNITY_EDITOR
        textureChanged = false;
        //CHANGED IF CHECKER
        //we check if texture changed since last update:
        if (chosentex != chosenTexture.GetValue<Texture2D>() && !Application.isPlaying && !firstrun)
        {
            //note: chosentex is a placedholder texture, it is not the final "maintexture"
            //if there was a change between what's store on twodTexture and twodTextureEditor
            //this part will update mainTexture
            textureChanged = true;
            chosentex = chosenTexture.GetValue<Texture2D>();
            //SetMainTexture is how the engine changes texture (it creates a new one in memory
            //so it can be editor at editing time, like clicking on Read/Write, but without the need to)
            myTwod.SetMainTexture(chosenTexture.GetValue<Texture2D>());
            //This will set the inital frame back to the position 0,0 and force
            //an "AnimationSet" recalculation. Needed later.
            myTwod.SetFrame(0, 0,true);
            
        }
#endif
        //this bool avoids to run last if on the first run, since chosentex will 
        //be always be null on first run, therefor triggering unwanted changes to the
        //maintexture. This happens when user select another asset un Unity's Editor
        //local Editor's version of chosentex will be updated later with the real inner chosentex
        firstrun = false;

        //One more check, in case that mainTexture is null, we set the cropped to null
        if (textureChanged && texture2d.GetValue<Texture2D>() == null)
        {
            myTwod.SetFrame(null);
        }
        
        //We get MainTexture and the ObjectField for it (local version of maintex is mainly
        //unused by now. Kept it as a shorthand for its values, checks "if changed" are done by chosentex)
        maintex = texture2d.GetValue<Texture2D>();

        //Now we draw the REAL control for chosentex ObjectField:
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Texture: ", GUILayout.MaxWidth(80f));        
        //this changes inner value of chosenTexture, but not local value of chosentex. Next
        //iteration "CHANGED IF CHECKER" will be triggered and will update inner maintexture.
        chosenTexture.SetValue<Texture2D>((Texture2D)EditorGUILayout.ObjectField(chosenTexture.GetValue<Texture2D>(), typeof(Texture2D), false));        
        EditorGUILayout.EndHorizontal();


        //Cellsize init
        //We initialize cellSize values now. Used Later on.
        Vector2Int cellSize = cellsize.GetValue<Vector2Int>();
        if (useDefault.GetValue<bool>())
        {
            twodController tc = twodController.instance;
            if (tc == null)
            {
                //if no instance, we show an error:
                EditorGUILayout.HelpBox("twodController is not present in the Scene!\nNo default values could be found!\nUse Default Cell Size will be OFF", MessageType.Error);
                useDef = false; //useDef turns to false so when we hit PLAY or another object it will go back to false
            }
            else
            {
                //there IS an instance of the twodController, we get the cellsize as an uneditable control
                cellSize = tc.cellSize;   
                
            }
        }
        Rect CellRect;
        int cellX =0;
        int cellY=0;
        #endregion

        //-----------------
        //Texture Preview
        //-----------------
        #region Texture Preview Part
        //If we HAVE a main texture:
        if (maintex != null)
        {
            //number of cells on X -1 (-1 to match array's)
            cellX = Mathf.RoundToInt(maintex.width / (float)cellSize.x - 1);
            //numer of cells on Y -1 (-1 to match array's)
            cellY = Mathf.RoundToInt(maintex.height / (float)cellSize.y - 1);




            //Draw Main texture preview            
            //Title:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Texture Preview:",GUILayout.MaxWidth(110f));
            EditorGUILayout.LabelField("Size: " + maintex.width.ToString() + "x" + maintex.height.ToString());
            EditorGUILayout.EndHorizontal();
            //We get the Last rect drawn by inspector (mainly for the position)
            ImageRect = GUILayoutUtility.GetLastRect();

            //We feed maintex width and height to our ApplyMaximuns. It will force resize of the image if needed (if too big)
            Vector2 size = ApplyMaximuns(maintex.width, maintex.height, imageclicked);
            //We modify ImageRect with the results of last statement
            //Note: ImageRect will be important one more time later on.
            ImageRect.x = 40f;
            ImageRect.y = ImageRect.y + ImageRect.height; //y is the position of last Rect drawn, we want the next position
            ImageRect.width = size.x; //we override with our custom size
            ImageRect.height = size.y; //we override with our custom size
            //ImageRect is now the exact Rect where the image should be. position and scale
            
            //If clicked it will get big on next frame. imageclicked gets fed into "ApplyMaximuns" next frame
            if (GUILayout.Button(imageclicked ? "-":"+", GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                imageclicked = !imageclicked;
                
            }
                        
            //We drawn the maintexture now on the rect
            EditorGUI.DrawPreviewTexture(new Rect(ImageRect.x, ImageRect.y, ImageRect.width+ 5f, ImageRect.height+ 5f), maintex);
            GUILayout.Space(ImageRect.height -20f );
        }
        else
        {
            //If we have no Texture, show an error instead!
            EditorGUILayout.HelpBox("No Texture Assigned", MessageType.Error);
        }
        #endregion

        //------------------
        //Cell Size Part
        //------------------
        #region Cell Size
        //we get the bool if we want to use default cellsize value
        //bool useDefsChanged = !(useDef == useDefault.GetValue<bool>());

        useDef = useDefault.GetValue<bool>();        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Title:
        EditorGUILayout.LabelField("Cell Size", EditorStyles.boldLabel);
        
        //We draw the control for the use of default cellsize here:
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Use Default Cell Size:");        
        useDefault.SetValue<bool>(EditorGUILayout.Toggle(useDef));
        EditorGUILayout.EndHorizontal();
        
        //if it's check (true)        
        if (useDef)
        {
            //We try to get the instance for the singleton (should be working on editor time, thanks to the twodController own Editor script, aswell as .instance property)
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
            EditorGUILayout.BeginHorizontal();
            Vector2Int tempv = cellsize.GetValue<Vector2Int>();

            //we draw the values of cellsize
            EditorGUILayout.LabelField("Cell Size: X: " + tempv.x.ToString() + " Y: " + tempv.y.ToString(),GUILayout.MaxWidth(160f),GUILayout.ExpandWidth(false));

            //As we dont want the editor to be constantly updating while user
            //enters numbers, we open this values on another window (cellSizeEditWindow)
            if (GUILayout.Button("Edit", GUILayout.MaxWidth(60f))){
                //we call the Static initializer for the window, which creates and opens it
                //also we pass the current value and the callback to update it when the user
                //clicks on accept
                cellSizeEditWindow.OpenWindow(tempv,(v2i) => { cellsize.SetValue<Vector2Int>(v2i); myTwod.SetFrame(0, 0, true); this.Repaint();  });

            }
            EditorGUILayout.EndHorizontal();
            //cellsize.SetValue<Vector2Int>(Vector2Int.RoundToVector2Int(EditorGUILayout.Vector2Field("Cell Size", cellsize.GetValue<Vector2Int>().toVector2())));
            //note: Vector2Int is a class created for twod which hold 2 integers
        }
        #endregion

        //--------------
        //Cell Preview
        //-------------
        #region Cell Preview Part
        //If we have no main texture, we dont draw anything
        if (maintex != null && cellSize != null && cellSize.x > 0 && cellSize.y > 0)
        {
            //we do Fail checks -> image wont be usable
            //FAIL CHECKS!
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

                

                //We draw Coordinates Back over TexturePreview
                #region Draw Coordinates
                //We draw X,Y coordinates on the previewTexture

                //We draw X cordinates
                float cellXOffset = ImageRect.width / ((float)cellX + 1);
                float cellYOffset = ImageRect.height / ((float)cellY + 1);
                float offset = 2.5f;
                if (imageclicked) offset = 2.5f;
                EditorGUILayout.BeginHorizontal();
                for (int x = -1; x <= cellX; x++)
                {
                    string str = x.ToString();
                    Rect littleRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
                    littleRect.y += littleRect.height + 5f;
                    littleRect.x += ((cellXOffset / 2) - offset) + ((cellXOffset + 0.5f) * x);
                    littleRect.width = cellXOffset;
                    littleRect.height = cellYOffset;
                    if (x == -1)
                    {
                        littleRect.width = 45f;
                        littleRect.x = 0f;
                        str = "Frames: ";
                    }


                    EditorGUI.LabelField(littleRect, str, EditorStyles.miniLabel);
                    //EditorGUILayout.LabelField(x.ToString(), EditorStyles.miniBoldLabel);

                }

                //We draw Y Coordinates
                EditorGUILayout.EndHorizontal();
                offset = 8f;
                List<twodAnimationSet> animasets = myTwod.AnimationSets;
                for (int y = 0; y <= cellY; y++)
                {
                    string str = y.ToString();
                    Rect littleRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
                    littleRect.y += ((cellYOffset / 2) - offset) + (y * (cellYOffset + 1f));
                    littleRect.x += littleRect.width + 10f;
                    littleRect.width = cellXOffset + 200f;
                    littleRect.height = cellYOffset;
                    bool setname = false;
                    if (animasets != null)
                    {
                        if (y <= animasets.Count - 1)
                        {
                            if (animasets[y] != null)
                            {
                                str = animasets[y].name;
                                setname = true;
                            }
                        }
                    }
                    if (!setname) str = "Animation " + y.ToString();
                    EditorGUI.LabelField(littleRect, str, EditorStyles.miniLabel);

                }
                #endregion

                #region Draw Selecction Marker
                //Selection marker
                Rect markerRect = new Rect(ImageRect.x, ImageRect.y, ImageRect.width, ImageRect.height);
                markerRect.x += (currentX * (cellXOffset + 1f));// + (cellXOffset / 3);
                markerRect.y += (currentY * (cellYOffset + 1f));// + (cellYOffset /3);
                markerRect.width = cellXOffset;
                markerRect.height = cellYOffset;
                //EditorGUI.LabelField(markerRect, "O");
                Color color = Color.grey;
                color.a = 0.5f;
                EditorGUI.DrawRect(markerRect, color);
                #endregion

                #region Draw Cell Buttons

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

                #endregion

                #region Draw CellTexture

                if (!checkValid(currentX,currentY,cellX,cellY)) { currentX = 0; currentY = 0; }

                Texture celltex = null;
                if (!failed) celltex =  myTwod.CropTexture(currentY, currentX);
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
                #endregion
            
            }
            #endregion



        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Finish and apply al changes
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



public class cellSizeEditWindow : EditorWindow
{
    public Vector2Int current = Vector2Int.zero;

    public Action<Vector2Int> OnAccept;
    static public void OpenWindow(Vector2Int currentcellsize, Action<Vector2Int> acceptCallback = null)
    {

        cellSizeEditWindow window = (cellSizeEditWindow)GetWindow(typeof(cellSizeEditWindow));
        window.OnAccept = acceptCallback;
        window.current = currentcellsize;
        window.titleContent.text = "Cell Size Editor";
        
        window.minSize = new Vector2(50, 30);
        window.maxSize = new Vector2(150, 100);
        window.Show();
        
        

    }


    


    void OnGUI()
    {
        current = Vector2Int.RoundToVector2Int(EditorGUILayout.Vector2Field("Cell Size", current.toVector2()));

        //Button create
        if (GUILayout.Button("Accept"))
        {
            

            //Vector2Int v2i = new Vector2Int(0, 0);
                if (OnAccept != null) OnAccept(current);
                this.Close();
            


        }

    }


}
