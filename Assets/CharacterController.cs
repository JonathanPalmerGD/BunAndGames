using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour 
{
	public float speed;
	public Vector3 targetPosition;
	private Vector3 clickPos;

	void Update () 
	{
		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * speed);

		clickPos = Input.mousePosition;

		if (Input.GetMouseButton (0))
		{
			clickPos.z = 10.0f;

			Vector3 targPoint = Camera.main.ScreenToWorldPoint (clickPos); //mousePosition returns Vector3 position based on screen width and heigh. 0,0 is bottom left corner.
			//Debug.DrawLine(Vector3.zero, targPoint, Color.white, 15.0f);
			Vector3 test = new Vector3((-Input.mousePosition.x - Screen.width/2), (Input.mousePosition.y - Screen.height/2), transform.position.z);

			targetPosition = targPoint;

			if(test.x > 0 && test.x < Screen.width && test.y > 0 && test.y < Screen.height){

				targetPosition = test;

			}
			//Debug.DrawLine(targetPosition, transform.position, Color.black, 15.0f);
		}

	


	}
}
