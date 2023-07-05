using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{   
    private static float[] screenResolution= {1920.0f, 1080.0f};

    public static float[] GetScreenResolution(){
        return screenResolution;
    }
    // Start is called before the first frame upd   ate
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
