using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour
{
    public abstract Vector2 Move(Enemy enemy, Player player);
}
