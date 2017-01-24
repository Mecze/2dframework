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
/// MainController for twod. It carries default and centralized configurations
/// </summary>

public class twodController : Singleton<twodController> {
    [SerializeField]
    [HideInInspector]
    public Vector2Int cellSize;

    [SerializeField]
    [HideInInspector]
    public float frameFrequency;
}
