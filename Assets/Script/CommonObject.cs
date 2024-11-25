using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonObject : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


}
