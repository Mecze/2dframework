/// <summary>
/// 
///     ######################
///     #                    #
///     #        TWOD        #
///     #  Created by Mecze  #
///     #                    #
///     ######################
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// twodAnimator
/// This Component manages animations between Frames
/// Requires twodTexture
/// </summary>

public delegate void GenericTwodEventHandler();


[RequireComponent(typeof(twodTexture))]
public class twodAnimator : MonoBehaviour {
    #region Inspector
    [Header("Animations")]

    /// <summary>
    ///If we should use Default Frame Frequency or custom (from this object)
    /// </summary>
    [SerializeField]    
    bool useDefaultFrameFrecuency;

    /// <summary>
    /// Custom Frame Frequency
    /// </summary>
    [HideInInspector]    
    float frameFrecuency {
        get {
            if (currentAnimation == null)
            {
                return twodController.instance.frameFrequency;
            }else
            {
                return currentAnimation.frameFrequency;
            }
        }
        set
        {
            if (currentAnimation != null) currentAnimation.frameFrequency = value;
        }
    }

    /// <summary>
    /// Public List of Animations
    /// </summary>
    public List<twodAnimation> Animations;

    #endregion

    #region Inner Variables and References
    /// <summary>
    /// Inner referencer to my twodTexture
    /// </summary>
    twodTexture myTwod;
    /// <summary>
    /// Dinamic reference to my twodTexture
    /// </summary>
    public twodTexture MyTwod
    {
        get
        {
            if (myTwod == null) myTwod = GetComponent<twodTexture>();
            if (myTwod == null) Debug.LogError("Cannot Access the twodTexture. Make sure a twodTexture is attached to this object");
            return myTwod;
        }        
    }

    /// <summary>
    /// Controls if an animation is playing. Set to Off to stop animations
    /// </summary>
    bool playing = false;

    /// <summary>
    /// Reference to current Animation configuration
    /// </summary>
    twodAnimation currentAnimation;


    int _currentFrame;
    float timer;
    private int currentFrame
    {
        get
        {
            return _currentFrame;
        }
        set
        {            
            int next = value;
            if (value > (currentAnimation.numberOfFrames - 1)) next = 0;
            if (value < 0) next = currentAnimation.numberOfFrames - 1;
            _currentFrame = next;
            MyTwod.SetFrame(currentAnimation.animationIndex, next);
        }
    }



    #endregion

    #region Events

    
    GenericTwodEventHandler _onChangedAnimation;
    /// <summary>
    /// Fires An event when the Animation Changes
    /// </summary>
    public event GenericTwodEventHandler OnChangedAnimation
    {
        add
        {
            if (_onChangedAnimation == null || !_onChangedAnimation.GetInvocationList().Contains(value))
            {
                _onChangedAnimation += value;
            }
            
        }
        remove
        {
            _onChangedAnimation -= value;
        }
    }

    
    GenericTwodEventHandler _onAnimatorStopped;
    /// <summary>
    /// Fires An event when the animator Stops
    /// </summary>
    public event GenericTwodEventHandler OnAnimatorStopped
    {
        add
        {
            if (_onAnimatorStopped == null || !_onAnimatorStopped.GetInvocationList().Contains(value))
            {
                _onAnimatorStopped += value;
            }
        }
        remove
        {
            _onAnimatorStopped -= value;
        }
    }

    #endregion


    #region Initialize
    /// <summary>
    /// Called from twodTexture, after its awake
    /// </summary>
    public void Initialize()
    {
        //Initalize FrameFrequency
        if (useDefaultFrameFrecuency || frameFrecuency == 0) frameFrecuency = twodController.instance.frameFrequency;
        
        //Initalize animation list        
        Animations = new List<twodAnimation>();
        for (int i = 0; i < MyTwod.AnimationSets.Count; i++)
        {
            Animations.Add(new twodAnimation(MyTwod.AnimationSets[i], frameFrecuency,this));
        }

        PlayAnimation(Animations[0]);
    }
    #endregion

    #region Animation Controls

    /// <summary>
    /// Play this Animation
    /// </summary>
    /// <param name="anim">Animation Object</param>
    /// <param name="NoEvents">Don't fire events for this transition</param>
    public void PlayAnimation(twodAnimation anim, bool NoEvents = false)
    {
        playing = true;
        if (currentAnimation != anim)
        {
            currentAnimation = anim;
            currentFrame = 0;
            if (_onChangedAnimation != null && NoEvents == false) _onChangedAnimation();
        }
    }
    /// <summary>
    /// Play animation by name
    /// </summary>
    /// <param name="animationName">Name of the animation to Play. eg "Animation 1", "Idle"....</param>
    /// <param name="NoEvents">Don't fire events for this transition</param>
    public void PlayAnimation(string animationName, bool NoEvents = false)
    {
        twodAnimation ta = Animations.Find(x => x.name == animationName);
        if (ta == null) { Debug.LogError("twodAnimator Cannot find the animation by name '" + animationName + "' on its List of Animations. Object: " + gameObject.name); return; }
        PlayAnimation(ta, NoEvents);
    }

    /// <summary>
    /// Stop from animating.
    /// </summary>
    /// <param name="NoEvents">Don't fire events for this transition</param>
    public void StopAnimation(bool NoEvents = false)
    {
        playing = false;
        currentFrame = 0;
        if (_onAnimatorStopped != null && NoEvents == false) _onAnimatorStopped();
    }
    #endregion
    
    #region Animate
    void Update()
    {
        Animate();
    }
    /// <summary>
    /// Animates if it has to.
    /// </summary>
    void Animate()
    {
        if (!playing) return; //we are not playing
        if (currentAnimation == null) return; //failsafe
        if (frameFrecuency == 0f) return; //failsafe
        if (currentFrame > (currentAnimation.numberOfFrames - 1) || currentFrame < 0) currentFrame = 0; //failsafe

        if (timer < Time.time)
        {
            timer = Time.time + frameFrecuency;
            currentFrame++;
        }
    }
    #endregion

    #region other methods
    /// <summary>
    /// Changes animator frecuency update rate
    /// this will update all animations frecuency rate to the new frecuency
    /// </summary>
    /// <param name="frecuency"></param>
    public void ChangeAnimatorFrecuency(float frecuency)
    {
        Animations.ForEach(x => x.frameFrequency = frecuency);
    }

    #endregion


}
