using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutTextFitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    this.GetComponent<RectTransform>().anchoredPosition = new Vector2(92.5f, this.GetComponent<RectTransform>().anchoredPosition.y);
    }
}
