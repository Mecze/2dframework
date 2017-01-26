using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
    public Texture2D test1;
    public Sprite test2;


    void Awake()
    {
        GetComponent<twodAnimator>().PlayAnimation("South");
    }
}
