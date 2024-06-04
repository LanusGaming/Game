using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuItem<T> : MonoBehaviour
{
    [Range(0f, 1f)]
    public float defaultAlpha = 0.5f;
    [Range(0f, 1f)]
    public float highlightAlpha = 1f;

    protected T value;

    private RectTransform bounds;
    private Graphic[] elements;

    protected virtual void Awake()
    {
        bounds = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = bounds.rect.size;
        collider.offset = bounds.rect.center;

        elements = GetComponentsInChildren<Graphic>();

        UpdateHighlight(false);
    }

    protected virtual void OnMouseEnter()
    {
        UpdateHighlight(true);
    }

    protected virtual void OnMouseExit()
    {
        UpdateHighlight(false);
    }

    private void UpdateHighlight(bool highlighted)
    {
        foreach (var element in elements)
        {
            Color color = element.color;
            color.a = highlighted ? highlightAlpha : defaultAlpha;
            element.color = color;
        }
    }

    public virtual T GetValue()
    {
        return value;
    }

    public virtual void SetValue(T value)
    {
        this.value = value;
        SettingsMenu.Updated(GetComponentInParent<ISettingsMenu>());
    }
}
