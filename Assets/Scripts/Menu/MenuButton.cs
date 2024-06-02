using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public RectTransform bounds;
    public Button.ButtonClickedEvent onClick;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = bounds.rect.size;
        collider.offset = bounds.rect.center;
    }

    private void OnMouseEnter()
    {
        animator.SetBool("Highlighted", true);
    }

    private void OnMouseExit()
    {
        animator.SetBool("Highlighted", false);
    }

    private void OnMouseDown()
    {
        animator.SetBool("Selected", true);
        onClick.Invoke();
    }

    public void Select()
    {
        animator.SetBool("Selected", true);
    }

    public void Deselect()
    {
        animator.SetBool("Selected", false);
    }
}
