using UnityEngine;
using System.Collections;

public class BunnyParticles : MonoBehaviour
{
	private ParticleSystem bunnyPart;
	ParticleSystem.Particle[] m_Particles;

	public GameObject Accelerator;

	void Start () 
	{
	
	}
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.X))
		{
			UpdateParticleColor(new Color(0.6f, 0.6f, 1.0f));
		}
		if(Input.GetKeyDown(KeyCode.C))
		{
			UpdateParticleColor(new Color(0.6f, 1.0f, 0.6f));
		}

		//HandleParticles();

		ClampParticleVel();
	}

	void HandleParticles()
	{
		InitializeIfNeeded();

		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			ClampIndividualParticle(m_Particles[i]);
		}

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	void AvoidParticle()
	{
		InitializeIfNeeded();

		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{

		}

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	void ClampIndividualParticle(ParticleSystem.Particle particle)
	{
		int bound = 10;

		particle.velocity = new Vector3(particle.velocity.x, 0, particle.velocity.z);

		//If particle would leave the screen
		if (particle.position.x < -bound && particle.velocity.x < 0)
		{
			particle.velocity = new Vector3(-particle.velocity.x, 0, particle.velocity.z);
		}

		//If particle would leave the screen
		if (particle.position.x > bound && particle.velocity.x > 0)
		{
			particle.velocity = new Vector3(-particle.velocity.x, 0, particle.velocity.z);
		}

		/*
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
		*/
		particle.velocity.Normalize();
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
