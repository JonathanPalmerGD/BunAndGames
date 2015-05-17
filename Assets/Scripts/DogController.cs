using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogController : MonoBehaviour 
{
	private int clickNum;
	private float speed = 10;
	public Vector3 targetPosition;
	private Vector3 mousePos;
	private int oldClicksAllowed = 300;
	public List<Vector3> clicks;
	private Vector3 mousePosLastFrame = Vector3.zero;
	private Vector3 mouseVelLastFrame = Vector3.zero;
	private Vector3 mouseVel = Vector3.zero;

	public bool animating = true;
	bool movedLastFrame = false;

	public Sprite left1;
	public Sprite left2;
	public Sprite right1;
	public Sprite right2;
	public Sprite sit1;
	public Sprite sit2;
	private float animTimer = 0;
	private float animRate = .15f;
	public LineRenderer lRend;
	public SpriteRenderer sprRend;

    public GameObject BlobPrefab;

	void Start()
	{
		clicks = new List<Vector3>();

		if (lRend != null)
		{
			lRend.SetWidth(.2f, .2f);
		}
	}

	void Update()
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

				if (!movedLastFrame)
				//if (xDif > -0.5f && xDif < 0.5f && yDif > -0.5f && yDif < 0.5f)
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

		if (GameManager.Inst.mode == GameManager.InputMode.Dog)
		{
			DogUpdate();
		}
		else if (GameManager.Inst.mode == GameManager.InputMode.Painting)
		{
			PaintUpdate();
		}
	}

	private void DogUpdate()
	{
		DrawPath();

		#region Position Updating
		if (clicks.Count > 0)
		{
			/*Debug.DrawLine(transform.position, clicks[0] - Vector3.forward, Color.cyan);
			for (int i = 1; i < clicks.Count; i++)
			{
				Debug.DrawLine(clicks[i - 1] - Vector3.forward, clicks[i] - Vector3.forward, Color.white);
			}*/

			targetPosition = clicks[0];
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
			transform.position += dir * Time.deltaTime * speed;

			if (clicks.Count > 0)
			{
				clicks.RemoveAt(0);
			}
		}

		if (dir.magnitude > 0.02)
		{
			movedLastFrame = true;
		}
		else
		{
			movedLastFrame = false;
		}

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
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
			{
				Vector3 targPoint = ScreenToWorldPos(mousePos);
				if (clicks.Count > oldClicksAllowed)
				{
					clicks.RemoveAt(0);
				}
				clicks.Add(targPoint);
			}
			else
			{
				if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0))
				{
					mouseVel = mousePosLastFrame - mousePos;
					Vector3 velDiff = mouseVel - mouseVelLastFrame;
					float magDiff = mouseVel.magnitude - mouseVelLastFrame.magnitude;

					//if (magDiff > 2 || magDiff < -2)
					//{
					Vector3 targPoint = ScreenToWorldPos(mousePos);
					if (clicks.Count > oldClicksAllowed)
					{
						clicks.RemoveAt(0);
					}
					clicks.Add(targPoint);
					//}
				}
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

	private void PaintUpdate()
	{
        #region Tapping to shoot
        mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
            RaycastHit hit;
            Vector3 _target = mousePos;
            Ray ray = Camera.main.ScreenPointToRay(_target);
            if (GameObject.Find("Grass Background").GetComponent<Collider>().Raycast(ray, out hit, 100.0F))
                _target = ray.GetPoint(100.0F);

			PaintBlob blob = ((GameObject)GameObject.Instantiate(BlobPrefab, gameObject.transform.position, Quaternion.identity)).GetComponent<PaintBlob>();
            _target.z = gameObject.transform.position.z;

			blob.colorIndex = GameManager.Inst.paintingIndex;
            Debug.Log(blob.colorIndex);
            blob.Approach(_target);
        }
        #endregion
    }

	private void DrawPath()
	{
		lRend.material.mainTextureScale = new Vector2(clicks.Count, 1);
		if (clicks.Count > 0)
		{
			lRend.SetVertexCount(clicks.Count + 1);
			lRend.SetPosition(0, transform.position);

			for (int i = 0; i < clicks.Count; i++)
			{
				lRend.SetPosition(i + 1, clicks[i]);
			}
		}
		else
		{
			lRend.SetVertexCount(0);
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
