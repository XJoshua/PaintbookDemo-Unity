using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class PaintGame : MonoBehaviour
{
    public static PaintGame Instance;

    public Transform GameItem;

    private SpriteRenderer[] gameSprites;
    private PaintItem[] items;
    private Color[] colorStrs;

    //public PolygonCollider2D[] ItemSprites;

    void Start()
    {
        Instance = this;

        colorStrs = new Color[GameItem.childCount];
        gameSprites = new SpriteRenderer[GameItem.childCount];
        items = new PaintItem[GameItem.childCount];

        ReadColor();

        for (int i = 0; i < GameItem.childCount; i++)
        {
            gameSprites[i] = GameItem.GetChild(i).GetComponent<SpriteRenderer>();
            items[i] = gameSprites[i].gameObject.AddComponent<PaintItem>();
            items[i].Init(colorStrs[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * 2, this.transform.localScale.y * 2, 1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * 0.5f, this.transform.localScale.y * 0.5f, 1);
        }
    }

    public void ReadColor()
    {
        var temp = Resources.Load<TextAsset>("paint00_config");

        Debug.Log(temp.text);

        //string info = (Resources.Load("paint00_config") as TextAsset).text;

        string[] result = temp.text.Split(',');

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = result[i].Replace(" ", "");
            result[i] = result[i].Replace("\"", "");
            result[i] = result[i].Replace("\"", "");

            Debug.Log(result[i]);
            colorStrs[i] = result[i].ToColor();
        }
    }
}

public static class StringEx
{
    // Example: "#ff000099".ToColor() red with alpha ~50%
    // Example: "ffffffff".ToColor() white with alpha 100%
    // Example: "00ff00".ToColor() green with alpha 100%
    // Example: "0000ff00".ToColor() blue with alpha 0%
    public static Color ToColor(this string color)
    {
        if (color.StartsWith("#", StringComparison.InvariantCulture))
        {
            color = color.Substring(1); // strip #
        }

        if (color.Length == 6)
        {
            color += "FF"; // add alpha if missing
        }

        var hex = Convert.ToUInt32(color, 16);
        var r = ((hex & 0xff000000) >> 0x18) / 255f;
        var g = ((hex & 0xff0000) >> 0x10) / 255f;
        var b = ((hex & 0xff00) >> 8) / 255f;
        var a = ((hex & 0xff)) / 255f;

        return new Color(r, g, b, a);
    }
}