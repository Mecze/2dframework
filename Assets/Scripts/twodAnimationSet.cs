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

/// <summary>
/// twodAnimationSets carries basic information about animations
/// </summary>
[System.Serializable]
public class twodAnimationSet {
    /// <summary>
    /// Reference to my twodTexture
    /// </summary>
    public twodTexture myTWOD;

    /// <summary>
    /// Name of the Animation
    /// </summary>
    [SerializeField]
    public string name;

    /// <summary>
    /// Number of Frames this animations has
    /// </summary>
    [SerializeField]
    public int numberOfFrames;

    /// <summary>
    /// Index of this animation (corresponding to the texture itself)
    /// </summary>
    public int animationIndex;


    #region Constructors
    /// <summary>
    /// Explicit Constructor
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="NumberOfFrames"></param>
    /// <param name="AnimationIndex"></param>
    /// <param name="MyTexture"></param>
    public twodAnimationSet(string Name, int NumberOfFrames, int AnimationIndex, twodTexture MyTexture) 
    {
        name = Name;
        numberOfFrames = NumberOfFrames;
        animationIndex = AnimationIndex;
        myTWOD = MyTexture;
    }   
    /// <summary>
    /// Empty Constructor
    /// </summary>
    public twodAnimationSet() :this("Animation",0,0,null) { } 
    /// <summary>
    /// Copy Constructor
    /// </summary>
    /// <param name="other"></param>
    public twodAnimationSet(twodAnimationSet other) : this(other.name, other.numberOfFrames, other.animationIndex, other.myTWOD) { }
    #endregion
}
