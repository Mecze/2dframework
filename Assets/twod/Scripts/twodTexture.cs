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
using System.Linq;

/// <summary>
/// twodTexture Manages one sprite Frame crops it by cellSize and store cropped cells as Sprites.
/// Requires an SpriteRenderer where to Show the Sprites
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public class twodTexture : MonoBehaviour {

    #region Inspector Variables

    [Header("Texture")]
    /// <summary>
    /// Main Texture
    /// </summary>
    [SerializeField]
    [HideInInspector]
    Texture2D mainTexture;

    /// <summary>
    /// GUI Texture Version of the Main Texture
    /// </summary>
    [SerializeField]
    [HideInInspector]
    Texture2D GUITexture;


    [SerializeField]
    [HideInInspector]
    Sprite ChosedSprite;

    [SerializeField]
    [HideInInspector]
    Texture2D ChosedTexture;


    /// <summary>
    /// Define if we use the Controller CellSize
    /// </summary>
    [SerializeField]
    [HideInInspector]
    bool useDefaultCellSize;

    /// <summary>
    /// The Size of each Cell
    /// </summary>
    [SerializeField]
    [HideInInspector]
    public Vector2Int cellSize;

    //[Header("Animations Sets")]
    [SerializeField]
    [HideInInspector]
    public List<twodAnimationSet> AnimationSets;

    /// <summary>
    /// Define if we use default FilterMode
    /// </summary>
    [SerializeField]
    [HideInInspector]
    bool useDefaultFilterMode = true;

    /// <summary>
    /// Define our current FilterMode
    /// </summary>
    [SerializeField]
    [HideInInspector]
    FilterMode myFilterMode;


    #endregion

    #region Inner Variables and references

    SpriteRenderer myRenderer;
    /// <summary>
    /// Dinamic reference to the renderer attached to this object.
    /// </summary>
    SpriteRenderer MyRenderer
    {
        get
        {
            if (myRenderer == null) myRenderer = GetComponent<SpriteRenderer>();
            if (myRenderer == null) Debug.LogError("twodTexture is Unable to access it's Sprite Renderer. Make sure it has one attached. Object: " + gameObject.name);
            return myRenderer;
        }
    }

    /// <summary>
    /// The top Pixel for inverting the Texture renderization
    /// </summary>
    //Texture conventions for graphics are: (0,0) is bottom left but we wanted to be top left, we need to calculate top part of the texture
    //So this is a shortcut to "Top"
    int topZero {
        get
        {
            if (useDefaultCellSize) return mainTexture.height- twodController.instance.cellSize.y;
            return mainTexture.height - cellSize.y;
        }
    }

    /// <summary>
    /// The main Sprite Store. It gets filled during Initialization
    /// </summary>
    [SerializeField]
    [HideInInspector]
    Sprite[,] SpriteStore;
    #endregion

    #region Event declaration and methods

#pragma warning disable 0649
    GenericTwodEventHandler[,] _frameEvents;
#pragma warning restore 0649

    /// <summary>
    /// Add an event to a certain Frame. 
    /// This will fire when said Frame starts.
    /// </summary>
    /// <param name="framePosition">The index of the frame</param>
    /// <param name="AnimationIndex">The Animation Index on the Texture</param>
    /// <param name="Event">The event. You can use { Anonymous }</param>
    /// <returns>returns if the operation was successful</returns>
    public bool AddEvent(int framePosition,int AnimationIndex, GenericTwodEventHandler Event)
    {
        if (_frameEvents[framePosition,AnimationIndex] == null || !_frameEvents[framePosition, AnimationIndex].GetInvocationList().Contains(Event))
        {
            _frameEvents[framePosition, AnimationIndex] += Event;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove an event from a certain frame.
    /// </summary>
    /// <param name="framePosition">The index of the frame</param>
    /// <param name="AnimationIndex">The Animation Index on the Texture</param>
    /// <param name="Event">The event. You can use { Anonymous }</param>
    /// <returns>returns if the operation was successful</returns>
    public bool RemoveEvent(int framePosition, int AnimationIndex, GenericTwodEventHandler Event)
    {
        if (_frameEvents[framePosition, AnimationIndex].GetInvocationList().Contains(Event))
        {
            _frameEvents[framePosition, AnimationIndex] -= Event;
            return true;
        }
        return false;

    }
    /// <summary>
    /// Removes ALL events from an specific Frame.
    /// (This is used to remove annonymous methods
    /// You can also "store" annonymous methods and remove them)
    /// </summary>
    /// <param name="framePosition">The index of the frame</param>
    /// <param name="AnimationIndex">The Animation Index on the Texture</param>
    public void ClearEvent(int framePosition,int AnimationIndex)
    {
        _frameEvents[framePosition, AnimationIndex] = null;
    }
    #endregion
    
    #region Initialization
    void Awake()
    {

        InitializeValues();        

        //Initial CROP        
        //SetFrame(0, 0);

        //Initalize animator (and other things)
        try
        {
            gameObject.SendMessage("Initialize");
        }
        catch (System.Exception)
        {

            throw;
        }
        
    }

    void InitializeValues()
    {
        //Set cellSize
        if (useDefaultCellSize) cellSize = twodController.instance.cellSize;
        if (cellSize.x == 0 || cellSize.y == 0) { if(Application.isPlaying)Debug.LogError("Cellsize X or Y in twodTexture is 0 nothing will be shown!. Object: " + gameObject.name); return; }

        //we warn the user that the texture is imperfect (This should be fixed!)
        
        if (mainTexture.width % cellSize.x != 0 && Application.isPlaying) Debug.LogError("Texture width and CellSize X are not on the correct format. there are leftover pixels at the end of the texture, this can lead to weirdness\nGame will try to play normally ignoring this pixels. \nObject: " + gameObject.name);
        if (mainTexture.height % cellSize.y != 0 && Application.isPlaying) Debug.LogError("Texture heigth and CellSize Y are not on the correct format. there are leftover pixels at the end of the texture, this can lead to weirdness\nGame will try to play normally ignoring this pixels. \nObject: " + gameObject.name);

        //we count how many cells we have on each direction
        int cellXCount = Mathf.FloorToInt(mainTexture.width / cellSize.x);
        int cellYCount = Mathf.FloorToInt(mainTexture.height / cellSize.y);

        //Initialize the list of animations        
        if (AnimationSets == null) AnimationSets = NewSets(cellYCount,cellXCount);

        //Initialize Store
        SpriteStore = new Sprite[cellXCount, cellYCount];

        List<twodAnimationSet> tempList = new List<twodAnimationSet>();
        //This FOR fills AnimationSets AND SpriteStore
        for (int i = 0; i < cellYCount; i++)
        {
            //we check AnimationSets Array.
            //It is possible that Animation Array wasnt created now, but was created before with
            //another texture, with another number of animations.
            //So we check if we have arrived to a point that this arrray doesnt have something in that index:
            if (AnimationSets.Count == i)
                //And we create it:
                AnimationSets.Add(new twodAnimationSet("Animation " + i.ToString(), cellXCount, i, this, false));
            
            //Now we check this item, if it was created by user, we override everything except for name and set by user
            if (AnimationSets[i].setByUser) AnimationSets[i] = new twodAnimationSet(AnimationSets[i].name, cellXCount, i, this, AnimationSets[i].setByUser);

            //To elimnate duplicates and old items we fill a temporary list
            tempList.Add(AnimationSets[i]); 
                //That's is how we conserve the names on the twodTextureEditor Animation Names.

            //Note: twodTexture.AnimationSets it's completly different to twodAnimator.Animations.
            //Animation Derive from AnimationSet but mechanicly the List twodAnimator.Animations is the important one.
            //twodTexture.AnimationsSets fullfils the purpose of user friendly inspector

            //Populate inner SpriteStore
            for (int e = 0; e < cellXCount; e++)
            {
                SpriteStore[e, i] = CropSprite(i, e);

            }
        }
        AnimationSets = tempList;
        

        //Initialize Event Array
        _frameEvents = new GenericTwodEventHandler[cellXCount, cellYCount];
    }

    List<twodAnimationSet> NewSets(int arrayLength, int numberOfFrames)
    {
        List<twodAnimationSet> r = new List<twodAnimationSet>();
        for (int i = 0; i < arrayLength; i++)
        {
            r.Add(new twodAnimationSet("Animation " + i.ToString(), numberOfFrames, i, this,false));
        }
        return r;
    }

    #endregion

    #region SetFrame
    /// <summary>
    /// This method changes myRenderer to the targeted sprite
    /// </summary>
    /// <param name="animation">the INDEX of the animation</param>
    /// <param name="frame">Frame</param>
    public void SetFrame(int animation, int frame,bool UpdateSpriteStore = false)
    {
        if (UpdateSpriteStore) InitializeValues();
        if (SpriteStore == null) return;
        if (SpriteStore.GetLength(0)== 0 || SpriteStore.GetLength(1) == 0 || SpriteStore.GetLength(0)<frame || SpriteStore.GetLength(1)<animation) return;
        MyRenderer.sprite = SpriteStore[frame, animation];
        if (_frameEvents[frame, animation] != null)
        {
            _frameEvents[frame, animation]();
        }
    }
    /// <summary>
    /// This method changes myRenderer to the targeted sprite
    /// </summary>
    /// <param name="animationName">the NAME of the animation</param>
    /// <param name="frame">Frame</param>
    public void SetFrame(string animationName, int frame)
    {
        twodAnimationSet tas = AnimationSets.Find(x => x.name == animationName);
        if (tas == null) { Debug.LogError("SetFrame tried to look for '" + animationName + "' on AnimationSets and didnt find it. Object: " + gameObject.name); return; }
        SetFrame(tas.animationIndex, frame);
    }

    /// <summary>
    /// This method changes myRenderer to the targeted sprite (Atomic version)
    /// </summary>
    /// <param name="sprite"></param>
    public void SetFrame(Sprite sprite)
    {        
        MyRenderer.sprite = sprite;
    }
    
        

    #endregion

    #region CROP
    /// <summary>
    /// This method finds and crops a portion of the frame. (called from initialization for each cell)
    /// </summary>
    /// <param name="animation">The animation it is in (Y)</param>
    /// <param name="frame">The frame it is in (X)</param>
    /// <returns></returns>
    public Sprite CropSprite(int animation, int frame, bool transparentToWhite = false)
    {
        if (mainTexture == null) return null;

        FilterMode fm = useDefaultFilterMode ? twodController.instance.filterMode : myFilterMode;

        Vector2Int thisVector = cellSize;
        if (useDefaultCellSize) thisVector = twodController.instance.cellSize;
        //Get pixel starting positions
        int posX = (thisVector.x * frame);
        int posY = topZero - (thisVector.y * animation);

        //Se handle possible errors
        if (posX < 0 || posX > mainTexture.width)
        {

            if (Application.isPlaying) Debug.LogError("Frame not Found when croping image, please check the frame. Object: " + gameObject.name);
            return null;
        }
        if (posY < 0 || posY > mainTexture.height)
        {
            if (Application.isPlaying) Debug.LogError("Animation not Found when croping image, please check the animation. Object: " + gameObject.name);
            return null;
        }
        if (posX + thisVector.x > mainTexture.width)
        {
            if (Application.isPlaying) Debug.LogError("Frame outside texture range, please Check Cell Size (X axis)\nIt might be too big. Object: " + gameObject.name);
            return null;
        }
        if (posY + thisVector.y > mainTexture.height)
        {
            if (Application.isPlaying) Debug.LogError("Frame outside texture range, please Check Cell Size (Y axis)\nIt might be too big.Object: " + gameObject.name);
            return null;
        }

        //We get an array of pixels from initial position and size of the cell. (crop)
        Color[] pixels = mainTexture.GetPixels(posX, posY, thisVector.x, thisVector.y);
        //We create a new texture to store the cropeed pixels
        Texture2D newCropped = new Texture2D(thisVector.x, thisVector.y,mainTexture.format,false,true);
        newCropped.filterMode = fm;
        newCropped.Apply();
        if (transparentToWhite)
        {
            for (int x = 0; x < pixels.Length; x++)
            {
                if (pixels[x].a == 0f)
                {
                    Vector2Int pos = twodUtils.ArrayToVector(x, newCropped.width);
                    bool white = (((pos.x - pos.y) - 4) & 4) == 4;

                    if (white)
                    {
                        pixels[x] = Color.white;
                    }
                    else
                    {
                        pixels[x] = Color.grey;
                        ColorUtility.TryParseHtmlString("#efefef", out pixels[x]);

                    }
                }
            }
        }



        //We now store the pixels into the new texture and apply for graphic card to update.
        newCropped.SetPixels(pixels);
        newCropped.Apply();
        //We create a new Sprite with our texture and return
        return Sprite.Create(newCropped, new Rect(0, 0, newCropped.width, newCropped.height), new Vector2(0.5f, 0.5f));
    }
    /// <summary>
    /// This method finds and crops a portion of the frame. (called from initialization for each cell)
    /// </summary>
    /// <param name="animation">The animation it is in (Y)</param>
    /// <param name="frame">The frame it is in (X)</param>
    /// <returns></returns>
    public Texture CropTexture(int animation, int frame, bool transparentToWhite = false)
    {
        if (mainTexture == null) return null;

        FilterMode fm = useDefaultFilterMode ? twodController.instance.filterMode : myFilterMode;
        //Get pixel starting positions
        Vector2Int thisVector = cellSize;
        if (useDefaultCellSize) thisVector = twodController.instance.cellSize;
       
        int posX = (thisVector.x * frame);
        int posY = topZero - (thisVector.y * animation);

        //handle possible errors
        if (posX < 0 || posX > mainTexture.width)
        {

            if (Application.isPlaying) Debug.LogError("Frame not Found when croping image, please check the frame. Object: " + gameObject.name);
            return null;
        }
        if (posY < 0 || posY > mainTexture.height)
        {
            if (Application.isPlaying) Debug.LogError("Animation not Found when croping image, please check the animation. Object: " + gameObject.name);
            return null;
        }
        if (posX + thisVector.x > mainTexture.width)
        {
            if (Application.isPlaying) Debug.LogError("Frame outside texture range, please Check Cell Size (X axis)\nIt might be too big. Object: " + gameObject.name);
            return null;
        }
        if (posY + thisVector.y > mainTexture.height)
        {
            if (Application.isPlaying) Debug.LogError("Frame outside texture range, please Check Cell Size (Y axis)\nIt might be too big.Object: " + gameObject.name);
        return null;
        }


        //We get an array of pixels from initial position and size of the cell. (crop)
        Color[] pixels;
        try
        {
            
            pixels = mainTexture.GetPixels(posX, posY, thisVector.x, thisVector.y);
        }
        catch
        {
            return null;
        }
        //We create a new texture to store the cropeed pixels
        Texture2D newCropped = new Texture2D(thisVector.x, thisVector.y, TextureFormat.ARGB32, true, true);
        //We now store the pixels into the new texture and apply for graphic card to update.
        if (thisVector.x <= 0 || thisVector.y <= 0) return null;

        if (transparentToWhite)
        {
            for (int x = 0; x < pixels.Length; x++)
            {
                if (pixels[x].a == 0f)
                {
                    Vector2Int pos = twodUtils.ArrayToVector(x, newCropped.width);
                    bool white = (((pos.x - pos.y) - 4) & 4) == 4;

                    if (white)
                    {
                        pixels[x] = Color.white;
                    }
                    else
                    {
                        pixels[x] = Color.grey;
                        ColorUtility.TryParseHtmlString("#efefef", out pixels[x]);

                    }
                }
            }
        }



        newCropped.filterMode = fm;
        newCropped.Apply();
        newCropped.SetPixels(pixels);

        

        newCropped.Apply();
        //We create a new Sprite with our texture and return
        return newCropped;
    }
    #endregion

    #region SetMainTExture

    /// <summary>
    /// Changes the mainTexture of this twodTexture
    /// </summary>
    /// <param name="texture"></param>
    public void SetMainTexture(Texture texture)
    {
        SetMainTexture((Texture2D)texture);
    }

    /// <summary>
    /// Changes the mainTexture of this twodTexture
    /// </summary>
    /// <param name="texture"></param>
    public void SetMainTexture(Texture2D texture)
    {
        if (texture == null) { mainTexture = null; return; }
        //sprite.texture.                

        FilterMode fm = useDefaultFilterMode ? twodController.instance.filterMode : myFilterMode;

        //we get pixels
        var pixels = texture.GetRawTextureData();

        //We calculate the and setup the MAIN texture
        Texture2D texture2d = new Texture2D((int)texture.width, (int)texture.height, texture.format, false, true);
        texture2d.filterMode = fm;
        texture2d.Apply();
        texture2d.LoadRawTextureData(pixels);
        texture2d.name = texture.name;
        texture2d.mipMapBias = 0f;                
        texture2d.alphaIsTransparency = true;
        texture2d.Apply();
        if (texture2d is Texture2D)
        {
            mainTexture = texture2d;
            //mainTexture = texture2d;
        }
        else
        {
            Debug.Log("Cannot Convert into Texture2D");
        }

        if (Application.isPlaying) return;


        Texture2D textureGUI = new Texture2D((int)texture2d.width, (int)texture2d.height, texture2d.format, false, true);
        textureGUI.filterMode = fm;
        textureGUI.Apply();
        Color[] pixels2 = texture2d.GetPixels();
        for (int x = 0; x < pixels2.Length; x++)
        {
            if (pixels2[x].a == 0f)
            {
                Vector2Int pos = twodUtils.ArrayToVector(x, texture2d.width);
                bool white = (((pos.x-pos.y) - 4) & 4) == 4;

                if (white)
                {
                    pixels2[x] = Color.white;
                }else
                {
                    pixels2[x] = Color.grey;
                    ColorUtility.TryParseHtmlString("#efefef", out pixels2[x]);
                    
                }
            }
        }
        textureGUI.SetPixels(pixels2);
        textureGUI.Apply();

        GUITexture = textureGUI;



    }
    /// <summary>
    /// Changes the mainTexture of this twodTexture
    /// </summary>
    /// <param name="texture"></param>
    public void SetMainTexture(Sprite sprite)
    {
        if (sprite == null) { mainTexture = null; return; }
        FilterMode fm = useDefaultFilterMode ? twodController.instance.filterMode : myFilterMode;
        //sprite.texture.
        Texture2D texture2d = new Texture2D((int)sprite.texture.width, (int)sprite.texture.height, sprite.texture.format, false);
        texture2d.Apply();
        //Texture2D texture2d = new Texture2D((int)sprite.texture.width, (int)sprite.texture.height,sprite.texture.format,false);
        var pixels = sprite.texture.GetRawTextureData();
        texture2d.filterMode = fm;
        texture2d.Apply();
        texture2d.LoadRawTextureData(pixels);
        texture2d.name = sprite.name;
        texture2d.Apply();
        if (texture2d is Texture2D)
        {
            mainTexture = texture2d;
        }
        else
        {
            Debug.Log("Cannot Convert Sprite into Texture2D");
        }



        if (Application.isPlaying) return;


        Texture2D textureGUI = new Texture2D((int)texture2d.width, (int)texture2d.height, texture2d.format, false, false);
        textureGUI.filterMode = fm;
        textureGUI.Apply();
        Color[] pixels2 = texture2d.GetPixels();
        for (int x = 0; x < pixels2.Length; x++)
        {
            if (pixels2[x].a == 0f)
            {
                Vector2Int pos = twodUtils.ArrayToVector(x, texture2d.width);
                bool white = (((pos.x - pos.y) - 4) & 4) == 4;

                if (white)
                {
                    pixels2[x] = Color.white;
                }
                else
                {
                    pixels2[x] = Color.grey;
                    ColorUtility.TryParseHtmlString("#efefef", out pixels2[x]);

                }
            }
        }
        textureGUI.SetPixels(pixels2);
        textureGUI.Apply();

        GUITexture = textureGUI;


    }



    #endregion

    

}
