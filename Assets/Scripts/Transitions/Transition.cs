using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public interface ILoadingIcon
{
    public void Show();
}

public class Transition : MonoBehaviour
{
    public Transform spriteMask;
    public float duration = 1;
    public bool reversed = false;
    public GameObject loadingIcon;

    public Action<Transition> doneCallback;

    private float timer;
    private float screenRadius;
    private bool done;

    void Start()
    {
        timer = 0;
        screenRadius = (new Vector2(Screen.width, Screen.height)).magnitude * Camera.main.orthographicSize/Screen.height * 2f + 1;
    }

    void Update()
    {
        if (reversed)
            spriteMask.localScale = Vector3.one * Mathf.Lerp(screenRadius, 0, timer / duration);
        else
            spriteMask.localScale = Vector3.one * Mathf.Lerp(0, screenRadius, timer / duration);

        if (timer < duration)
            timer += Time.deltaTime;
        else if (!done)
        {
            done = true;

            if (reversed)
            {
                if (loadingIcon)
                {
                    GameObject loadingIconObject = Instantiate(loadingIcon, transform);
                    loadingIconObject.GetComponent<ILoadingIcon>().Show();
                }
            }

            doneCallback.Invoke(this);
        }
    }
    
    
}
