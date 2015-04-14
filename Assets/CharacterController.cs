using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour 
{
	public float speed;
	public Vector3 targetPosition;
	private Vector3 clickPos;

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

	void Update () 
	{
		if (animating)
		{
			animTimer += Time.deltaTime;
			//Change frame
			if (animTimer > animRate)
			{
				animTimer = 0;

				float xDif = targetPosition.x - transform.position.x;

				if (xDif < -0.5f)
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
				else if (xDif > 0.5f)
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
				else
				{
					//sprRend.sprite = sit1;

					if (sprRend.sprite != sit1)
					{
						sprRend.sprite = sit1;
					}
					else
					{
						sprRend.sprite = sit2;
					}

				}
			}
		}

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
