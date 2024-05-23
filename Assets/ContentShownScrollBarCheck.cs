using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentShownScrollBarCheck : MonoBehaviour
{
	private Scrollbar scroll;
	[SerializeField] Cards cards;
	
	private bool canLoad = true;
	
    // Start is called before the first frame update
    void Start()
    {
	    scroll = this.GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void ValueChange() 
	{
		if (scroll.value < 0.03f && canLoad) 
		{
			cards.IncreaseShown();
			canLoad = false;
			StartCoroutine("WaitForLoad");
		}
	}
	
	IEnumerator WaitForLoad() 
	{
		yield return new WaitForSeconds(0.5f);
		canLoad = true;
	}
}
