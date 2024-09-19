using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObjects with this script won't be destroyed when the scene changes
/// </summary>
public class GenericSingleton : MonoBehaviour
{
    public static GenericSingleton Instance;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
