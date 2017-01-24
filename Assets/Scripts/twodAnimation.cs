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
/// twodAnimation carries information of one animation. Expands twodAnimationSet information.
/// </summary>

[System.Serializable]
public class twodAnimation : twodAnimationSet {
    #region Constructors
    /// <summary>
    /// Explicit Constructor
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="NumberOfFrames"></param>
    /// <param name="AnimationIndex"></param>
    /// <param name="MyTexture"></param>
    /// <param name="FrameFrequency"></param>
    public twodAnimation(string Name, int NumberOfFrames, int AnimationIndex, twodTexture MyTexture,float FrameFrequency, twodAnimator MyAnimator) : base(Name,NumberOfFrames,AnimationIndex,MyTexture) 
    {
        frameFrequency = FrameFrequency;
        myAnimator = MyAnimator;
    }
    /// <summary>
    /// COPY constructor
    /// </summary>
    /// <param name="other"></param>
    public twodAnimation(twodAnimation other) : this(other.name, other.numberOfFrames, other.animationIndex, other.myTWOD, other.frameFrequency, other.myAnimator) { }
    /// <summary>
    /// COPY from twodAnimationSet Constructor
    /// </summary>
    /// <param name="mySet"></param>
    /// <param name="FrameFrequency"></param>
    public twodAnimation(twodAnimationSet mySet, float FrameFrequency, twodAnimator MyAnimator) : this(mySet.name, mySet.numberOfFrames, mySet.animationIndex, mySet.myTWOD, FrameFrequency, MyAnimator) { }
  
    /// <summary>
    /// Empty Constructor
    /// </summary>
    public twodAnimation() : this("", 0, 0, null,0f, null){}
    #endregion   


    [SerializeField]
    [Range(0.0f, 3f)]
    public float frameFrequency;
    [SerializeField]
    public twodAnimator myAnimator;

   
}

