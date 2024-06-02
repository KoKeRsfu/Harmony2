using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling.Memory;

public class MemorySnaps : MonoBehaviour
{
	
	public void TakeMemorySnapshot() 
	{
		MemoryProfiler.TakeSnapshot(Application.persistentDataPath, null, CaptureFlags.ManagedObjects);
	}
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	protected void Start()
	{
		StartCoroutine("test1");
	}
	
	public IEnumerator test1() 
	{
		Debug.Log("1");
		
		yield return test2();
		
		Debug.Log("4");
	}
	
	public IEnumerator test2()
	{
		Debug.Log("2");
		
		yield return test3();
	}
	
	public IEnumerator test3() 
	{	
		yield return new WaitForSeconds(0.1f);
		
		Debug.Log("3");
	}
}
