using UnityEngine;

/// <summary>
/// Custom Vector2 Class with Integers
/// </summary>
[System.Serializable]
public class Vector2Int
{

    public static Vector2Int zero
    {
        get
        {
            return new Vector2Int(0, 0);
        }
    }

    #region Inner Variables
    /// <summary>
    /// Inner x
    /// </summary>
    [SerializeField]
    int _x;

    /// <summary>
    /// Inner y
    /// </summary>
    [SerializeField]
    int _y;
    #endregion

    #region Properties
    /// <summary>
    /// x int units
    /// </summary>
    public int x
    {
        get
        {
            return _x;
        }

        set
        {
            _x = value;
        }
    }

    /// <summary>
    /// y int units
    /// </summary>
    public int y
    {
        get
        {
            return _y;
        }

        set
        {
            _y = value;
        }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Custom amount constructo
    /// </summary>
    /// <param name="x">X Integer</param>
    /// <param name="y">Y Integer</param>
    public Vector2Int(int x, int y)
    {
        _x = x;
        _y = y;
    }

    /// <summary>
    /// Default Consturctor (0,0)
    /// </summary>
    public Vector2Int()
    {
        _x = 0;
        _y = 0;
    }
    #endregion

    public Vector2 toVector2()
    {
        return new Vector2((float)_x, (float)_y);
    }

    public static Vector2Int RoundToVector2Int(Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }
    public static Vector2Int FloorToVector2Int(Vector2 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }
    public override string ToString()
    {
        return "x: " + _x.ToString() + " y: " + _y.ToString();


    }
}
