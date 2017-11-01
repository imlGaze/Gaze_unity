using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye {

    public enum EyeType
    {
        LEFT,
        RIGHT,
    };

    public EyeType type;
    public int x, y;

    public Eye(EyeType type, int x, int y)
    {
        this.type = type;
        this.x = x;
        this.y = y;
    }

    private void Start()
    {
        
    }
}

