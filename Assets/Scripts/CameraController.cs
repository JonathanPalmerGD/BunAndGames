using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public GameObject background;
	public GameObject camSizeBg;

	private float halfScaleX;
	private float halfScaleY;

	private bool checkX = true;
	private bool checkZ = true;

	private float offsetX;
	private float offsetZ;

	void Start ()
	{
		halfScaleX = Mathf.Abs(background.transform.localScale.x/2);
		halfScaleY = Mathf.Abs(background.transform.localScale.y/2);
	}

	void Update () 
	{
		// always use 16:9
		offsetX = (Mathf.Abs(player.transform.position.x)) + (camSizeBg.transform.localScale.x/2);
		offsetZ = (Mathf.Abs(player.transform.position.z)) + (camSizeBg.transform.localScale.y/2);

		if(offsetX < halfScaleX) // inside boundary x
		{
			checkX = true;
		}
		else // outside boundary x
		{
			checkX = false;
		}

		if(offsetZ < halfScaleY) // inside boundary z or y
		{
			checkZ = true;
		}
		else // inside boundary z or y
		{
			checkZ = false;
		}

		if(checkX == true && checkZ == true)
		{
			this.transform.position = new Vector3(player.transform.position.x,this.transform.position.y,player.transform.position.z);
		}
		else if(checkX == true && checkZ == false)
		{
			this.transform.position = new Vector3(player.transform.position.x,this.transform.position.y,this.transform.position.z);
		}
		else if(checkX == false && checkZ == true)
		{
			this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,player.transform.position.z);
		}
		else
			this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,this.transform.position.z);
	}
}