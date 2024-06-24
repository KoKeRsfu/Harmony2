using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	    Invoke("Hide", 2f);	
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void Hide()
	{
		this.gameObject.SetActive(false);	
	}
	
}
