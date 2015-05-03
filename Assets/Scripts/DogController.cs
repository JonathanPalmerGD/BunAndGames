using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogController : MonoBehaviour 
{
	private int clickNum;
	private float speed = 10;
	public Vector3 targetPosition;
	private Vector3 mousePos;
	private Touch[] touches;
	private int oldClicksAllowed = 100;
	public List<Vector3> clicks;
	private Vector3 mousePosLastFrame = Vector3.zero;
	private Vector3 mouseVelLastFrame = Vector3.zero;
	private Vector3 mouseVel = Vector3.zero;

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

	void Start()
	{
		clicks = new List<Vector3>();
		touches = new Touch[2];
	}

	void Update()
	{
		#region Position Updating
		if (clicks.Count > 0)
		{
			Debug.DrawLine(transform.position, clicks[0] - Vector3.forward, Color.cyan);
			for (int i = 1; i < clicks.Count; i++)
			{
				Debug.DrawLine(clicks[i - 1] - Vector3.forward, clicks[i] - Vector3.forward, Color.white);
			}
				
			targetPosition = clicks[0];
		}
		#endregion
		
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
	
		#region Position & Click updating
		Vector3 dir = targetPosition - transform.position;
		//Debug.Log(dir.magnitude);
		if (dir.magnitude > 1)
		{
			dir.Normalize();
			transform.position += dir * Time.deltaTime * speed;
		}
		else
		{
			if (clicks.Count > 0)
			{
				clicks.RemoveAt(0);
			}
		}
		//transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * speed);

		mouseVelLastFrame = mouseVel;
		mousePosLastFrame = mousePos;
		mousePos = Input.mousePosition; //mousePosition returns Vector3 position based on screen width and heigh. 0,0 is bottom left corner.
		mousePos.z = 10;

		if (Input.touchCount != null)
		{
			clickNum = Input.touchCount;
		}
		#endregion
		if (true)
		{
			#region Swiping
			mouseVel = mousePosLastFrame - mousePos;
			Vector3 velDiff = mouseVel - mouseVelLastFrame;
			float magDiff = mouseVel.magnitude - mouseVelLastFrame.magnitude;

			if (magDiff > 2 || magDiff < -2)
			{
				Vector3 targPoint = ScreenToWorldPos(mousePos);
				if (clicks.Count > oldClicksAllowed)
				{
					clicks.RemoveAt(0);
				}
				clicks.Add(targPoint);
			}
			#endregion
		}
		else
		{
			#region Tapping for new Destinations
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 targPoint = ScreenToWorldPos(mousePos);
				if (clicks.Count > oldClicksAllowed)
				{
					clicks.RemoveAt(0);
				}
				clicks.Add(targPoint);
			}
			#endregion
		}
	}

	private Vector3 ScreenToWorldPos(Vector3 mousePos)
	{
		mousePos.z = 10;
		Vector3 targPoint = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 adjustedMousePos = new Vector3((-Input.mousePosition.x - Screen.width / 2), (Input.mousePosition.y - Screen.height / 2), transform.position.z);

		/*if (adjustedMousePos.x > 0 && adjustedMousePos.x < Screen.width && adjustedMousePos.y > 0 && adjustedMousePos.y < Screen.height)
		{
				
		}*/

		return targPoint;
	}
}
