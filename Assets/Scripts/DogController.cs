using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour 
{
	private int clickNum;
	public float speed;
	public Vector3 targetPosition;
	private Vector3 clickPos;
	private Touch[] touches;

	public bool animating = true;

	public Sprite left1;
	public Sprite left2;
	public Sprite right1;
	public Sprite right2;
	public Sprite sit1;
	public Sprite sit2;
	private float animTimer = 0;
	private float animRate = .15f;
	public SpriteRenderer sprRend;

	void Start(){

		touches = new Touch[2];

	}

	void Update () 
	{
		#region Animation
		if (animating)
		{
			animTimer += Time.deltaTime;
			//Change frame
			if (animTimer > animRate)
			{
				animTimer = 0;

				float xDif = targetPosition.x - transform.position.x;
				float yDif = targetPosition.y - transform.position.y;

				if (xDif > -0.5f && xDif < 0.5f && yDif > -0.5f && yDif < 0.5f)
				{
					if (sprRend.sprite != sit1)
					{
						sprRend.sprite = sit1;
					}
					else
					{
						sprRend.sprite = sit2;
					}
				}
				else if (xDif < 0.0f)
				{
					if (sprRend.sprite != left1)
					{
						sprRend.sprite = left1;
					}
					else
					{
						sprRend.sprite = left2;
					}
				}
				else if (xDif > 0.0f)
				{
					if (sprRend.sprite != right1)
					{
						sprRend.sprite = right1;
					}
					else
					{
						sprRend.sprite = right2;
					}
				}
			}
		}
		#endregion

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * speed);

		clickPos = Input.mousePosition; //mousePosition returns Vector3 position based on screen width and heigh. 0,0 is bottom left corner.

		if (Input.touchCount != null)
			clickNum = Input.touchCount;

		//Debug.Log ("Touch Count: " + clickNum);

		if (Input.GetMouseButton (0)) {

			if (Input.GetMouseButtonDown (0) && (Input.touchCount == null || Input.touchCount <= 0)){
				clickNum += 1;
				/*for (int i = 0; i < clickNum; i++){
					//touches[i] = Input.GetTouch();
					//Debug.Log("Touches array? " + touches.GetLength());
				}*/
				//Debug.Log ("Click Count: " + clickNum);
			}

			clickPos.z = 10.0f;

			Vector3 targPoint = Camera.main.ScreenToWorldPoint (clickPos); 
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
