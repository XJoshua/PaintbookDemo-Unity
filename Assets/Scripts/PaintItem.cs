using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintItem : MonoBehaviour
{
    private Color color;

    public void Init(Color col)
    {
        color = col;
    }

    public void OnMouseDown()
    {
        this.GetComponent<SpriteRenderer>().color = color;
    }
}
