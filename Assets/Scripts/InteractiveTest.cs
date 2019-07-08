using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log(this.gameObject.name);

        this.GetComponent<SpriteRenderer>().color = Color.red;
    }


}
