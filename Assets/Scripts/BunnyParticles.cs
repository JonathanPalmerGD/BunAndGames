﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BunnyParticles : MonoBehaviour
{
	public ParticleSystem bunnyPart;
	ParticleSystem.Particle[] m_Particles;

	public GameObject dog;

	public string[] bunnyInfo;
	public int[] bunnyDir;
	private float normalFleeRange = 1.5f;
	private float fleeRange = 1.5f;
	private float fearStrength = 2f;
	private float maxVelocity = 2;
	public int HowManyBunnies;
	public AudioSource bark;

	public Camera cam;

	public Vector3 paintPoint;
	private float paintCounter = 0;
	private float paintTimer = 0.35f;
	public Color paintColor = new Color(.7f, .0f, .0f);
	private float paintChangeRate = 10;

	public Color[] colorOptions;
	public int paintIndex;

	private Vector3 clickPos;

	public void Init()
	{
		InitializeIfNeeded();
		bunnyPart.maxParticles = HowManyBunnies;
		bunnyInfo = new string[HowManyBunnies];
		bunnyDir = new int[HowManyBunnies];

		colorOptions = new Color[25];
		for (int i = 0; i < colorOptions.Length; i++)
		{
			colorOptions[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
		}
		cam = GameObject.Find("Main Camera").camera;

		Invoke("RandomParticleColor", 1f + HowManyBunnies / 900);
	}

	void Update()
	{
		#region Paint clock
		if (paintCounter > 0)
		{
			paintCounter -= Time.deltaTime;
		}
		#endregion

		#region Paint At Cursor
		if (Input.GetKey(KeyCode.Q) || Input.GetMouseButton(1))
		{
			clickPos = Input.mousePosition;
			clickPos.z = 10.0f;

			Vector3 targPoint = Camera.main.ScreenToWorldPoint (clickPos);
			Vector3 mousePointOnScreen = new Vector3((-Input.mousePosition.x - Screen.width/2), (Input.mousePosition.y - Screen.height/2), transform.position.z);

			paintPoint = targPoint;

			if(mousePointOnScreen.x > 0 && mousePointOnScreen.x < Screen.width && mousePointOnScreen.y > 0 && mousePointOnScreen.y < Screen.height)
			{
				paintPoint = mousePointOnScreen;
			}

			paintCounter = paintTimer;

			Debug.DrawLine(paintPoint, new Vector3(0, 0, -10), Color.white, 15.0f);
		}
		#endregion

		#region Color Changing
		if (paintColor == colorOptions[paintIndex])
		{
			paintIndex = Random.Range(0, colorOptions.Length);
		}
		else
		{
			paintColor = new Color(
				Mathf.Lerp(paintColor.r, colorOptions[paintIndex].r, Time.deltaTime * paintChangeRate), 
				Mathf.Lerp(paintColor.g, colorOptions[paintIndex].g, Time.deltaTime * paintChangeRate), 
				Mathf.Lerp(paintColor.b, colorOptions[paintIndex].b, Time.deltaTime * paintChangeRate));
		}
		#endregion

		#region Old Mass Paint
		/*if(Input.GetKeyDown(KeyCode.X))
		{
			UpdateParticleColor(new Color(0.6f, 0.6f, 1.0f));
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			UpdateParticleColor(new Color(0.6f, 1.0f, 0.6f));
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			RandomParticleColor();
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			//bunnyPart.maxParticles = bunnyPart.maxParticles * 2;
		}*/
		#endregion

		#region Barking
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Bark();
		}

		fleeRange = Mathf.Lerp(fleeRange, normalFleeRange, Time.deltaTime * 10);
		#endregion

		HandleParticles();

		//ClampParticleVel();
	}

	public void Bark()
	{
		fleeRange = 6;

		bark.Play();
	}

	void HandleParticles()
	{
		InitializeIfNeeded();

		int xBound = 9;
		int yBound = 6;
		
		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			#region Screen bounding

			m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);

			//If particle would leave the screen
			if (m_Particles[i].position.x < -xBound && m_Particles[i].velocity.x < 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.x > xBound && m_Particles[i].velocity.x > 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
			}


			//If particle would leave the screen
			if (m_Particles[i].position.y < -yBound && m_Particles[i].velocity.y < 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, -m_Particles[i].velocity.y, 0);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.y > yBound && m_Particles[i].velocity.y > 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, -m_Particles[i].velocity.y, 0);
			}
			#endregion

			#region Old Screen Bounding
			/*m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);

			//If particle would leave the screen
			if (m_Particles[i].position.x < -bound && m_Particles[i].velocity.x < 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.x > bound && m_Particles[i].velocity.x > 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);
			}


			//If particle would leave the screen
			if (m_Particles[i].position.z < -bound && m_Particles[i].velocity.z < 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, -m_Particles[i].velocity.z);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.z > bound && m_Particles[i].velocity.z > 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, -m_Particles[i].velocity.z);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.z < -bound && m_Particles[i].velocity.z < 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, -m_Particles[i].velocity.z);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.z > bound && m_Particles[i].velocity.z > 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, -m_Particles[i].velocity.z);
			}*/

			//m_Particles[i].velocity.Normalize();
			#endregion

			#region Bunny Painting
			if (paintCounter > 0)
			{
				float dist = Vector3.Distance(paintPoint, m_Particles[i].position);
				if (dist < 1.25f)
				{
					m_Particles[i].color = paintColor;
				}
			}
			#endregion

			#region Bunny Scaring
			if (dog != null)
			{
				Vector3 dogPos = new Vector3(dog.transform.position.x, dog.transform.position.y, 0);

				float distToDog = Vector3.Distance(m_Particles[i].position, dogPos);
				if (distToDog < fleeRange)
				{
					Vector3 fearVector = dogPos - m_Particles[i].position;
					//Debug.DrawLine(dogPos, dogPos - fearVector, Color.black, .5f);
					m_Particles[i].velocity -= fearVector;
					m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
					m_Particles[i].velocity.Normalize();
					if (distToDog < .2f)
					{
						distToDog = .2f;
					}
					float accel = (fearStrength / distToDog);

					m_Particles[i].velocity = m_Particles[i].velocity * accel;
					m_Particles[i].velocity.Normalize();
				}
			}
			#endregion

			#region Direction Storing
			//StoreVelocity
			bunnyInfo[i] = m_Particles[i].velocity.ToString();
			//Vector3 curVel = m_Particles[i].velocity;

			//Directional
			//	7	0	1
			//	6	+	2
			//	5	4	3

			float xVel = m_Particles[i].velocity.x;
			float yVel = m_Particles[i].velocity.y;

			bool set = false;

			if (xVel > 0)
			{
				if (xVel > Mathf.Abs(yVel))
				{
					bunnyDir[i] = 2;
					set = true;
				}
			}
			else
			{
				if(xVel < -Mathf.Abs(yVel))
				{
					bunnyDir[i] = 6;
					set = true;
				}
			}
			if (!set)
			{
				if (yVel > 0)
				{
					if (yVel > Mathf.Abs(xVel))
					{
						bunnyDir[i] = 0;
					}
				}
				else
				{
					if (yVel < -Mathf.Abs(xVel))
					{
						bunnyDir[i] = 4;
					}
				}
			}

			#region Diagonal Directions
			/*
			if (m_Particles[i].velocity.x > 0)
			{
				if (m_Particles[i].velocity.y > 0)
				{
					bunnyDir[i] = 1;
				}
				else
				{
					bunnyDir[i] = 3;
				}
			}
			else
			{
				if (m_Particles[i].velocity.y > 0)
				{
					bunnyDir[i] = 7;
				}
				else
				{
					bunnyDir[i] = 5;
				}
			}*/
			#endregion
			#endregion

			#region Epoch Locking

			float maxAge = 20 - bunnyDir[i] * .555555555f;
			float minAge = 20 - (bunnyDir[i] + 1) * .555555555f;

			//If the current lifetime is above the MAX lifetime, set it to the max lifetime
			if (m_Particles[i].lifetime >= maxAge)
			{
				m_Particles[i].lifetime = maxAge;
			}
			else if(m_Particles[i].lifetime <= minAge + .15f)
			{
				m_Particles[i].lifetime = maxAge - .15f;
			}
			//If the current lifetime is below the MIN lifetime, set it to the min lifetime









			/*
			if (m_Particles[i].lifetime < m_Particles[i].startLifetime / bunnyDir[i])
			{
				m_Particles[i].lifetime = m_Particles[i].startLifetime / bunnyDir[i] + 2;
			}
			else if (m_Particles[i].lifetime > m_Particles[i].startLifetime / bunnyDir[i] + 2)
			{
				m_Particles[i].lifetime = m_Particles[i].startLifetime / bunnyDir[i] + 2;
			}*/
			#endregion

			#region Velocity capping
			bunnyInfo[i] += "   " + m_Particles[i].velocity.magnitude;

			if (m_Particles[i].velocity.magnitude > 10)
			{
				//m_Particles[i].color = Color.black;
				m_Particles[i].velocity = m_Particles[i].velocity.normalized * 5;
			}
			else
			{
				float mag = m_Particles[i].velocity.magnitude;
				m_Particles[i].velocity = m_Particles[i].velocity.normalized * Mathf.Lerp(mag, maxVelocity, Time.deltaTime);
			}
			#endregion
		}
		
		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	void ClampParticleVel()
	{
		InitializeIfNeeded();

		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);
		int bound = 10;

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);

			//If particle would leave the screen
			if (m_Particles[i].position.x < -bound && m_Particles[i].velocity.x < 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.x > bound && m_Particles[i].velocity.x > 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);
			}


			//If particle would leave the screen
			if (m_Particles[i].position.z < -bound && m_Particles[i].velocity.z < 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, -m_Particles[i].velocity.z);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.z > bound && m_Particles[i].velocity.z > 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, -m_Particles[i].velocity.z);
			}

			m_Particles[i].velocity.Normalize();
		}

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	void RandomParticleColor()
	{
		InitializeIfNeeded();

		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			m_Particles[i].color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
		}

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	void UpdateParticleColor(Color newColor)
	{
		InitializeIfNeeded();

		particleSystem.startColor = newColor;

		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			m_Particles[i].color = newColor;
		}

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	public void ClearBunnies()
	{
		bunnyPart.maxParticles = 0;
	}

	void InitializeIfNeeded()
	{
		if (bunnyPart == null)
		{
			bunnyPart = GetComponent<ParticleSystem>();
		}
		if (m_Particles == null || m_Particles.Length < bunnyPart.maxParticles)
		{
			m_Particles = new ParticleSystem.Particle[bunnyPart.maxParticles];
		}
	}
}
