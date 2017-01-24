using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<Instance> : MonoBehaviour where Instance : Singleton<Instance> {
    
    static Instance _instance;
    public static Instance instance
    {
        set { _instance = value; }
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Instance>();
            }
            return _instance;
        }
    }
    [SerializeField]
    public bool isPersistent;

    public virtual void Awake()
    {
        if (isPersistent)
        {
            if (!instance)
            {
                instance = this as Instance;
            }
            else
            {
                DestroyObject(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance = this as Instance;
        }
    }
}
