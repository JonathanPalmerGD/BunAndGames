using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public GameObject background;
	public Vector3 futurePosition;
	public bool set = false;

	// SCREEN DIMENSIONS IN WORLD COORDS
	private float screenHIWC; // SCREEN HEIGHT IN WORLD COORDS
	private float screenWIWC; // SCREEN WIDTH IN WORLD COORDS

	private Vector3 backgroundTR; // BACKGROUND TOP RIGHT CORNER
	private Vector3 backgroundLL; // BACKGROUND LOWER LEFT CORNER

	void Start () {
	}

	private Vector3[] getGameBounds(Vector3[] meshVerts) {
		Vector3[] bounds = new Vector3[4];
		
		// [0] = least x, [1] = greatest x, [2] = greatest y, [3] = least y
		float[] values = new float[4];

		for (int i = 0; i < values.Length; i++) {
			values[i] = 0;
		}
		for (int i = 0; i < meshVerts.Length; i++) { 
			float x= meshVerts[i].x, y= meshVerts[i].y;
			if (x > values[1]) {
				values[1] = x;
			}
			if (x < values[0]) {
				values[0] = x;
			}
			if (y > values[2]) {
				values[2] = y;
			}
			if (y < values[3]) {
				values[3] = y;
			}
		}

		bounds[0] = new Vector3(values[0], values[3], 0);
		bounds[1] = new Vector3(values[0], values[2], 0);
		bounds[2] = new Vector3(values[1], values[2], 0);
		bounds[3] = new Vector3(values[1], values[3], 0);
		return bounds;
	}

	void Update () {
		if (!set) {
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
			

			Vector3 lower_left =
				lowerLeft;
			print("screen " + lower_left + "\n");

			Vector3 upper_right =
				upperRight;
			print("screen " + upper_right + "\n");

			screenHIWC = Mathf.Abs(upper_right.y - lower_left.y);
			screenWIWC = Mathf.Abs(upper_right.x - lower_left.x);

			Vector3[] bounds = getGameBounds(background.GetComponent<MeshFilter>().mesh.vertices);
			backgroundLL = bounds[0];
			backgroundTR = bounds[2];
			set = true;
		}

		futurePosition = player.gameObject.transform.position;
		futurePosition.z = this.gameObject.transform.position.z;
		
		// X LOCATION LOCKED IN RANGE DESIRED
		if (futurePosition.x > backgroundTR.x - (screenWIWC / 2)) {
			futurePosition.x = backgroundTR.x - (screenWIWC / 2); 
		}
		if (futurePosition.x < backgroundLL.x + (screenWIWC / 2)) {
			futurePosition.x = backgroundLL.x + (screenWIWC / 2); 
		}

		// Y LOCATION LOCKED IN RANGE DESIRED
		if (futurePosition.y > backgroundTR.y - (screenHIWC / 2)) {
			futurePosition.x = backgroundTR.y - (screenHIWC / 2);
		}
		if (futurePosition.y < backgroundLL.y + (screenHIWC / 2)) {
			futurePosition.y = backgroundLL.y + (screenHIWC / 2);
		}

		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,futurePosition,0.05f);
	}
}