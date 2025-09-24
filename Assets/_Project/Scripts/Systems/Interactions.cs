using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : SingletonCom<Interactions>
{
    public bool PlayerIsDragging { get; set; } = false;

    public bool PlayerCanInteract()
    {
        return true;
    }

    public bool PlayerCanHover()
    {
        if (PlayerIsDragging) return false;

        return true;
    }
}