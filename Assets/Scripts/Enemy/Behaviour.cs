using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VersionControl.Git.ICSharpCode.SharpZipLib.Tar;
using UnityEngine;

public abstract class Behaviour : MonoBehaviour
{
    public abstract Vector2 Move(Enemy enemy, Player player);
}
