using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour 
{
	public GameObject prefab;
	public int counter = 0;

	void Start () 
	{
	
	}
	
	void Update () 
	{
		if(Input.GetKey(KeyCode.Q))
		{
			counter++;

			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);

			GameObject.Instantiate(prefab, pos, Quaternion.identity);
		}

		if(Input.GetKey(KeyCode.T))
		{
			for(int i = 0; i < 15; i++)
			{
				counter++;
				
				Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), 0);
				
				GameObject.Instantiate(prefab, pos, Quaternion.identity);
			}
		}

		
		if(Input.GetKey(KeyCode.E))
		{
			Debug.Log("There are currently: " + counter + " sprites\n"); 
		}
	}
}
