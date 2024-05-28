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
}
