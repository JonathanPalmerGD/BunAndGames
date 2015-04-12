using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BunnyParticles : MonoBehaviour
{
	private ParticleSystem bunnyPart;
	ParticleSystem.Particle[] m_Particles;

	public GameObject dog;

	public string[] bunnyInfo;
	public int[] bunnyDir;

	public GameObject Accelerator;

	void Start ()
	{
		bunnyInfo = new string[10000];
		bunnyDir = new int[10000];
	}

	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.X))
		{
			UpdateParticleColor(new Color(0.6f, 0.6f, 1.0f));
		}
		if(Input.GetKeyDown(KeyCode.C))
		{
			UpdateParticleColor(new Color(0.6f, 1.0f, 0.6f));
		}

		HandleParticles();

		//ClampParticleVel();
	}

	void HandleParticles()
	{
		InitializeIfNeeded();

		int bound = 10;
		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			#region Screen bounding
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
			#endregion

			Vector3 dogPos = new Vector3(dog.transform.position.x, 0, dog.transform.position.y);

			float distToDog = Vector3.Distance(m_Particles[i].position, dogPos);
			if(distToDog < 5)
			{
				m_Particles[i].velocity -= dog.transform.position - m_Particles[i].position;
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, 0, m_Particles[i].velocity.z);
				m_Particles[i].velocity.Normalize();
				float accel = (2 / distToDog);

				//TODO:
				//Cap the accel

				m_Particles[i].velocity = m_Particles[i].velocity * accel;
			}

			//StoreVelocity
			//bunnyInfo[i] = m_Particles[i].velocity.ToString();
			Vector3 curVel = m_Particles[i].velocity;

			//Directional
			//	7	0	1
			//	6	+	2
			//	5	4	3

			if (m_Particles[i].velocity.x > 0)
			{
				if (m_Particles[i].velocity.z > 0)
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
				if (m_Particles[i].velocity.z > 0)
				{
					bunnyDir[i] = 7;
				}
				else
				{
					bunnyDir[i] = 5;
				}
			}

			if (m_Particles[i].lifetime < m_Particles[i].startLifetime / bunnyDir[i])
			{
				m_Particles[i].lifetime = m_Particles[i].startLifetime / bunnyDir[i] + 2;
			}
			else if (m_Particles[i].lifetime > m_Particles[i].startLifetime / bunnyDir[i] + 2)
			{
				m_Particles[i].lifetime = m_Particles[i].startLifetime / bunnyDir[i] + 2;
			}
			
			//bunnyInfo[i] = m_Particles[i].velocity.ToString() + "  " + m_Particles[i].lifetime;


			//m_Particles[i].lifetime = .5f;
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
