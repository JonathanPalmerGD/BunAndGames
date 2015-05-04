using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public GameObject background;
	public Vector3 targetPosition;
	public bool initialized = false;

	// SCREEN DIMENSIONS IN WORLD COORDS
	public float screenHIWC; // SCREEN HEIGHT IN WORLD COORDS
	public float screenWIWC; // SCREEN WIDTH IN WORLD COORDS

	public Vector3 lowerLeftHC; // BACKGROUND LOWER LEFT CORNER
	public Vector3 topRightHC; // BACKGROUND TOP RIGHT CORNER
	public Vector3 upper_right, lower_left;

	void Init() {
		Vector3 myScreen = new Vector3(Screen.width, Screen.height, 0);

		// Screens coordinate corner location
		var upperLeftScreen = new Vector3(0, Screen.height, Camera.main.depth);
		var upperRightScreen = new Vector3(Screen.width, Screen.height, Camera.main.depth);
		var lowerLeftScreen = new Vector3(0, 0, Camera.main.depth);
		var lowerRightScreen = new Vector3(Screen.width, 0, Camera.main.depth);

		//Corner locations in world coordinates
		var upperLeft = camera.ScreenToWorldPoint(upperLeftScreen);
		var upperRight = camera.ScreenToWorldPoint(upperRightScreen);
		var lowerLeft = camera.ScreenToWorldPoint(lowerLeftScreen);
		var lowerRight = camera.ScreenToWorldPoint(lowerRightScreen);

		lower_left = lowerLeft;
		upper_right = upperRight;

		screenHIWC = Mathf.Abs(upper_right.y - lower_left.y);
		screenWIWC = Mathf.Abs(upper_right.x - lower_left.x);

		topRightHC = background.transform.localScale / 2;
		lowerLeftHC = -background.transform.localScale / 2;
		initialized = true;
	}

	void Update () {
		if (!initialized) {
			Init();
		}

		Debug.DrawLine(new Vector3(lowerLeftHC.x, lowerLeftHC.y, -5),
					   new Vector3(topRightHC.x, topRightHC.y, -5), Color.cyan);
		Debug.DrawLine(new Vector3(upper_right.x,upper_right.y, -3),
					   new Vector3(lower_left.x,lower_left.y, -3), Color.red);

		targetPosition = player.gameObject.transform.position;
		//Keep the camera far out
		targetPosition.z = this.gameObject.transform.position.z;
		
		// X LOCATION LOCKED IN RANGE DESIRED
		if (targetPosition.x > topRightHC.x - (screenWIWC / 2)) {
			targetPosition.x = topRightHC.x - (screenWIWC / 2); 
		}
		if (targetPosition.x < lowerLeftHC.x + (screenWIWC / 2)) {
			targetPosition.x = lowerLeftHC.x + (screenWIWC / 2); 
		}

		// Y LOCATION LOCKED IN RANGE DESIRED
		if (targetPosition.y > topRightHC.y - (screenHIWC / 2)) {
			targetPosition.y = topRightHC.y - (screenHIWC / 2);
		}
		if (targetPosition.y < lowerLeftHC.y + (screenHIWC / 2)) {
			targetPosition.y = lowerLeftHC.y + (screenHIWC / 2);
		}

		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, 0.05f);
	}
}