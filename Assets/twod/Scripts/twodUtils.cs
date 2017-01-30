using UnityEngine;
using System.Collections;

public static class twodUtils  {


    /// <summary>
    /// Transform an array position into a position in an Image
    /// Used to locate a "GetPixels" pixel on its parent image
    /// </summary>
    /// <param name="positionInArray">the position of the value on the array</param>
    /// <param name="width">The width of the image</param>
    /// <returns></returns>
	public static Vector2Int ArrayToVector(int positionInArray,int width)
    {
        Vector2Int pos = new Vector2Int(0, 0);
        int x = positionInArray;
        while (x >= width)
        {
            pos.y++;
            x -= width;
        }
        pos.x = x;

        return pos;
    }

}

