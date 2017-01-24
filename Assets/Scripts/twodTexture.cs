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
        if (cellSize.x == 0 && cellSize.y == 0) Debug.LogError("Cellsize in twodTexture is 0,0 nothing will be shown!. Object: " + gameObject.name);

        //we warn the user that the texture is imperfect (This should be fixed!)
        if (mainTexture.width % cellSize.x != 0) Debug.LogError("Texture width and CellSize X are not on the correct format. there are left over pixels at the end of the texture, this can lead to weirdness. Object: " + gameObject.name);
        if (mainTexture.height % cellSize.y != 0) Debug.LogError("Texture heigth and CellSize Y are not on the correct format. there are left over pixels at the end of the texture, this can lead to weirdness. Object: " + gameObject.name);

        //we count how many cells we have on each direction
        int cellXCount = Mathf.FloorToInt(mainTexture.width / cellSize.x);
        int cellYCount = Mathf.FloorToInt(mainTexture.height / cellSize.y);

        //Initialize the list of animations
        AnimationSets = new List<twodAnimationSet>();

        //Initialize Store
        SpriteStore = new Sprite[cellXCount, cellYCount];

        //This FOR fills AnimationSets AND SpriteStore
        for (int i = 0; i < cellYCount; i++)
        {
            //Populate AnimationSets
            AnimationSets.Add(new twodAnimationSet("Animation " + i.ToString(), cellXCount, i, this));

            //Populate inner SpriteStore
            for (int e = 0; e < cellXCount; e++)
            {
                SpriteStore[e, i] = CropSprite(i, e);

            }
        }
        //Initialize Event Array
        _frameEvents = new GenericTwodEventHandler[cellXCount, cellYCount];
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
    public Sprite CropSprite(int animation, int frame)
    {
        //Get pixel starting positions
        int posX = (cellSize.x * frame);
        int posY = topZero - (cellSize.y * animation);

        //Se handle possible errors
        if (posX < 0 || posX > mainTexture.width)
        {
            Debug.LogError("Frame not Found when croping image, please check the frame. Object: " + gameObject.name);
            return null;
        }
        if (posY < 0 || posY > mainTexture.height)
        {
            Debug.LogError("Animation not Found when croping image, please check he animation. Object: " +gameObject.name);
            return null;
        }

        //We get an array of pixels from initial position and size of the cell. (crop)
        Color[] pixels = mainTexture.GetPixels(posX, posY, cellSize.x, cellSize.y);
        //We create a new texture to store the cropeed pixels
        Texture2D newCropped = new Texture2D(cellSize.x, cellSize.y);
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
    public Texture CropTexture(int animation, int frame)
    {
        if (mainTexture == null) return null;
        //Get pixel starting positions
        int posX = (cellSize.x * frame);
        int posY = topZero - (cellSize.y * animation);

        //Se handle possible errors
        if (posX < 0 || posX > mainTexture.width)
        {

            if (Application.isPlaying) Debug.LogError("Frame not Found when croping image, please check the frame. Object: " + gameObject.name);
            return null;
        }
        if (posY < 0 || posY > mainTexture.height)
        {
            if (Application.isPlaying) Debug.LogError("Animation not Found when croping image, please check he animation. Object: " + gameObject.name);
            return null;
        }

        //We get an array of pixels from initial position and size of the cell. (crop)
        Color[] pixels;
        try
        {
            pixels = mainTexture.GetPixels(posX, posY, cellSize.x, cellSize.y);
        }
        catch
        {
            return null;
        }
        //We create a new texture to store the cropeed pixels
        Texture2D newCropped = new Texture2D(cellSize.x, cellSize.y);
        //We now store the pixels into the new texture and apply for graphic card to update.
        if (cellSize.x <= 0 || cellSize.y <= 0) return null;

        newCropped.SetPixels(pixels);
        newCropped.Apply();
        //We create a new Sprite with our texture and return
        return newCropped;
    }
    #endregion

}
