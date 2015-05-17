using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BunnyParticles : MonoBehaviour
{
	#region Object references
	public GameObject dog;
	public CameraController camCon;
	public Camera cam;
	public AudioSource bark;
    public List<PaintBlob> Blobs;
	#endregion

	#region Particle Info
	public ParticleSystem bunnyPart;
	ParticleSystem.Particle[] m_Particles;
	public string[] bunnyInfo;
	public int[] bunnyDir;
	public int[] bunnyColor;
	private float normalFleeRange = 1.5f;
	private float fleeRange = 1.5f;
	private float fearStrength = 2f;
	private float maxVelocity = 2;
	public int HowManyBunnies;
	public bool collisions = false;
	#endregion

	#region Thread vs Normal
	bool switching = false;
	bool printing = false;
	#endregion

	#region Color Variables
	public bool forcePalette = true;
	public Vector3 paintPoint;
	private float paintCounter = 0;
	private float paintTimer = 0.35f;
	public Color paintColor = new Color(.7f, .0f, .0f);
	private float paintChangeRate = 10;


	private Color[] PaletteColor = { new Color( .270f, .807f, .937f),
									new Color( 1.00f, .960f, .647f),
									new Color( 1.00f, .831f, .854f),
									new Color( .600f, .823f, .894f),
									new Color( .847f, .792f, .705f), };

	public Color[] colorOptions;
	public int paintIndex;
	private bool paletteChanged = false;
	#endregion

    private Vector3 clickPos;
    private List<Obstacle> obs;

	public void Init()
	{
		InitializeIfNeeded();
		bunnyPart.maxParticles = HowManyBunnies;
		bunnyInfo = new string[HowManyBunnies];
		bunnyDir = new int[HowManyBunnies];
		bunnyColor = new int[HowManyBunnies];

		obs = new List<Obstacle>();
        Blobs = new List<PaintBlob>();
        PaintBlob.bunnies = this;
		UnityEngine.Object[] objects = GameObject.FindGameObjectsWithTag("Obstacle");

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Obstacle"))
		{
			Obstacle indObs = go.GetComponent<Obstacle>();
			if (indObs != null)
			{
				obs.Add(indObs);
			}
		}

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
		if (Input.GetKey(KeyCode.Z) || Input.GetMouseButton(1))
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

			//Debug.DrawLine(paintPoint, new Vector3(0, 0, -10), Color.white, 15.0f);
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

		if(Input.GetKeyDown(KeyCode.I))
		{
			switching = !switching;
		}
		if(switching)
		{
			HandleParticles();
		}	
		else
		{
			ThreadAssistHandleParticles();
			StartCoroutine("BunnyVelAndCol");
		}
		
		if(Input.GetKeyDown(KeyCode.U))
		{
			printing = !printing;
		}
		if(printing)
		{
			if(switching)
			{
				Debug.Log("Non threaded\n");
			}
			else	
			{
				Debug.Log("Threaded\n");
			}
		}
		//ClampParticleVel();
	}

	public void Bark()
	{
		GameManager.Inst.GainPoints(2);

		fleeRange = 6;

		bark.Play();
	}

	#region Handle Particles
	void HandleParticles()
	{
		InitializeIfNeeded();
		
		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		Vector3 dogPos = new Vector3(dog.transform.position.x, dog.transform.position.y , 0);
		Vector3 fearVector = Vector3.zero;
		Vector3 obsPos;
		float dist;
		float prevVel;
		float distFromObj = float.MaxValue;
		float xVel, yVel;
		float accel;
		bool set;
		float lifeTime, cycles, maxAge, minAge, ageAdjust;
		
		//float largestAdjust = 0, smallestAdjust = 0;

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			#region Big Screen Bounding
			//lowerLeftHC;
			//topRightHC;

			m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);

			//If particle would leave the screen
			if (m_Particles[i].position.x < camCon.lowerLeftHC.x + 1 && m_Particles[i].velocity.x < 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.x > camCon.topRightHC.x - 1 && m_Particles[i].velocity.x > 0)
			{
				m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
			}


			//If particle would leave the screen
			if (m_Particles[i].position.y < camCon.lowerLeftHC.y + 1 && m_Particles[i].velocity.y < 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, -m_Particles[i].velocity.y, 0);
			}

			//If particle would leave the screen
			if (m_Particles[i].position.y > camCon.topRightHC.y - 1 && m_Particles[i].velocity.y > 0)
			{
				m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, -m_Particles[i].velocity.y, 0);
			}
			#endregion

			#region Bunny Painting
			if (paintCounter > 0)
			{
				dist = Vector3.Distance(paintPoint, m_Particles[i].position);
				if (dist < 1.25f)
				{
					m_Particles[i].color = PaletteColor[bunnyColor[i]];
					//m_Particles[i].color = paintColor;
				}
			}

			#endregion

			#region Palette Color Changing
			if (paletteChanged)
			{
				m_Particles[i].color = PaletteColor[bunnyColor[i]];
				/*dist = Vector3.Distance(paintPoint, m_Particles[i].position);
				if (dist < 1.25f)
				{
					m_Particles[i].color = PaletteColor[bunnyColor[i]];
					//m_Particles[i].color = paintColor;
				}*/
			}
			#endregion

			#region Bunny Scaring
			if (dog != null)
			{
				distFromObj = Vector3.Distance(m_Particles[i].position, dogPos);
				if (distFromObj < fleeRange)
				{
					GameManager.Inst.GainPointsTimeRate(3);

					fearVector = dogPos - m_Particles[i].position;

					//Debug.DrawLine(dogPos, dogPos - fearVector, Color.black, .5f);
					m_Particles[i].velocity -= fearVector;
					m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
					m_Particles[i].velocity.Normalize();
					if (distFromObj < .2f)
					{
						distFromObj = .2f;
					}
					accel = (fearStrength / distFromObj);

					m_Particles[i].velocity = m_Particles[i].velocity * accel;
					m_Particles[i].velocity.Normalize();
				}
			}

			#endregion

			#region Walls
			if (obs != null)
			{
				for (int j = 0; j < obs.Count; j++)
				{
					distFromObj = Vector3.Distance(m_Particles[i].position, obs[j].transform.position);

					if (distFromObj < obs[j].radius)
					{
						prevVel = m_Particles[i].velocity.magnitude;

						obsPos = new Vector3(obs[j].transform.position.x + m_Particles[i].velocity.x * Time.deltaTime, obs[j].transform.position.y + m_Particles[i].velocity.y * Time.deltaTime, 0);
						fearVector = obsPos - m_Particles[i].position;

						m_Particles[i].velocity -= fearVector;
						m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
						m_Particles[i].velocity.Normalize();

						Debug.DrawLine(m_Particles[i].position, m_Particles[i].position + m_Particles[i].velocity.normalized, Color.green, 5.0f);
						//m_Particles[i].velocity = m_Particles[i].velocity;
						//m_Particles[i].velocity.Normalize();
					}
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

			xVel = m_Particles[i].velocity.x;
			yVel = m_Particles[i].velocity.y;

			set = false;

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

			#region Age Adjustment based on velocity magnitude

			//	Epoch Locking - Age Adjustment
			//			by Jonathan Palmer

			//Age Adjust is how much we need to adjust the age of this particle this frame.

			//	Step 1: Divide the magnitude by 100
			//	Step 2: Allow for the animation to be slowed, subtract a small amount
			//	Step 3: Clamp it between a min value (to prevent stopping or reversing animation)
			//	Step 4: Clamp it between a max value (to prevent it from skipping too many frames)
			ageAdjust = Mathf.Clamp(m_Particles[i].velocity.magnitude / 100 - .02f, -.01f, .2f);


			//	Final Step: Alter the particle lifetime.
			m_Particles[i].lifetime -= ageAdjust;

			//The particles are handed back into the particle system at the end of BunnyParticles.HandleParticle()

			/*if (ageAdjust > largestAdjust)
			{
				largestAdjust = ageAdjust;
			}
			if (ageAdjust < smallestAdjust)
			{
				smallestAdjust = ageAdjust;
			}*/
			#endregion

			#region Epoch Locking
			//			By Jonathan Palmer

			//Side note: bunnyDir is a CPU side int array the size of how many bunny particles we have.
			//	The int in the array at each indexed location represents the direction the bunny is moving
			//	Bunny Direction int: (0-7, starting 0 for north and incrementing clockwise)

			//Total particle Lifetime
			lifeTime = bunnyPart.startLifetime;

			//How many cycles it is set to animate.
			cycles = 8;

			//Unfortunately, we're calculating this every frame.
			//Future improvement: Calculate this once for each direction.
			maxAge = lifeTime - (bunnyDir[i] * 8 * cycles / lifeTime);
			minAge = lifeTime - ((bunnyDir[i] + 1) * 8 * cycles / lifeTime);
			
			//If the current lifetime is above the MAX lifetime, set it to the max lifetime
			if (m_Particles[i].lifetime >= maxAge)
			{
				m_Particles[i].lifetime = maxAge;
			}
			//If the current lifetime is below the MIN lifetime, set it to the min lifetime
			else if(m_Particles[i].lifetime <= minAge + .15f)
			{
				m_Particles[i].lifetime = maxAge - .15f;
			}
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

		//Debug.Log("Largest: " + largestAdjust + "\nSmallest: " + smallestAdjust);

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

	void ThreadAssistHandleParticles()
	{
		InitializeIfNeeded();
		
		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		Vector3 dogPos = new Vector3(dog.transform.position.x, dog.transform.position.y, 0);
		Vector3 fearVector = Vector3.zero;
		//Vector3 obsPos;
		float dist;
		//float prevVel;
		//float distFromObj = float.MaxValue;
		float xVel, yVel;
		//float accel;
		bool set;
		float lifeTime, cycles, maxAge, minAge, ageAdjust;
		
		//float largestAdjust = 0, smallestAdjust = 0;

		// Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            #region Bunny Painting
			if (paletteChanged || paintCounter > 0)
            {
				m_Particles[i].color = PaletteColor[bunnyColor[i]];
				/*dist = Vector3.Distance(paintPoint, m_Particles[i].position);
				if (dist < 1.25f)
				{
					m_Particles[i].color = PaletteColor[bunnyColor[i]];
					//m_Particles[i].color = paintColor;
				}*/
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

            xVel = m_Particles[i].velocity.x;
            yVel = m_Particles[i].velocity.y;

            set = false;

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
                if (xVel < -Mathf.Abs(yVel))
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

            #region Age Adjustment based on velocity magnitude

            //	Epoch Locking - Age Adjustment
            //			by Jonathan Palmer

            //Age Adjust is how much we need to adjust the age of this particle this frame.

            //	Step 1: Divide the magnitude by 100
            //	Step 2: Allow for the animation to be slowed, subtract a small amount
            //	Step 3: Clamp it between a min value (to prevent stopping or reversing animation)
            //	Step 4: Clamp it between a max value (to prevent it from skipping too many frames)
            ageAdjust = Mathf.Clamp(m_Particles[i].velocity.magnitude / 100 - .02f, -.01f, .2f);


            //	Final Step: Alter the particle lifetime.
            m_Particles[i].lifetime -= ageAdjust;

            //The particles are handed back into the particle system at the end of BunnyParticles.HandleParticle()

            /*if (ageAdjust > largestAdjust)
            {
                largestAdjust = ageAdjust;
            }
            if (ageAdjust < smallestAdjust)
            {
                smallestAdjust = ageAdjust;
            }*/
            #endregion

            #region Epoch Locking
            //			By Jonathan Palmer

            //Side note: bunnyDir is a CPU side int array the size of how many bunny particles we have.
            //	The int in the array at each indexed location represents the direction the bunny is moving
            //	Bunny Direction int: (0-7, starting 0 for north and incrementing clockwise)

            //Total particle Lifetime
            lifeTime = bunnyPart.startLifetime;

            //How many cycles it is set to animate.
            cycles = 8;

            //Unfortunately, we're calculating this every frame.
            //Future improvement: Calculate this once for each direction.
            maxAge = lifeTime - (bunnyDir[i] * 8 * cycles / lifeTime);
            minAge = lifeTime - ((bunnyDir[i] + 1) * 8 * cycles / lifeTime);

            //If the current lifetime is above the MAX lifetime, set it to the max lifetime
            if (m_Particles[i].lifetime >= maxAge)
            {
                m_Particles[i].lifetime = maxAge;
            }
            //If the current lifetime is below the MIN lifetime, set it to the min lifetime
            else if (m_Particles[i].lifetime <= minAge + .15f)
            {
                m_Particles[i].lifetime = maxAge - .15f;
            }
            #endregion
        }

		//Debug.Log("Largest: " + largestAdjust + "\nSmallest: " + smallestAdjust);

		// Apply the particle changes to the particle system
		bunnyPart.SetParticles(m_Particles, numParticlesAlive);
	}

    IEnumerator BunnyVelAndCol() {
        // Change only the particles that are alive
        int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

        Vector3 dogPos = new Vector3(dog.transform.position.x, dog.transform.position.y, 0);
        Vector3 fearVector = Vector3.zero;
        Vector3 obsPos;
        float prevVel;
        float distFromObj = float.MaxValue;
        float accel;
        for (int i = 0; i < numParticlesAlive; i++) {
            #region Big Screen Bounding
            //lowerLeftHC;
            //topRightHC;

            m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);

            //If particle would leave the screen
            if (m_Particles[i].position.x < camCon.lowerLeftHC.x + 1 && m_Particles[i].velocity.x < 0)
            {
                m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
            }

            //If particle would leave the screen
            if (m_Particles[i].position.x > camCon.topRightHC.x - 1 && m_Particles[i].velocity.x > 0)
            {
                m_Particles[i].velocity = new Vector3(-m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
            }


            //If particle would leave the screen
            if (m_Particles[i].position.y < camCon.lowerLeftHC.y + 1 && m_Particles[i].velocity.y < 0)
            {
                m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, -m_Particles[i].velocity.y, 0);
            }

            //If particle would leave the screen
            if (m_Particles[i].position.y > camCon.topRightHC.y - 1 && m_Particles[i].velocity.y > 0)
            {
                m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, -m_Particles[i].velocity.y, 0);
            }
            #endregion
            #region Bunny Scaring
            if (dog != null)
            {
                distFromObj = Vector3.Distance(m_Particles[i].position, dogPos);
                if (distFromObj < fleeRange)
                {
                    GameManager.Inst.GainPointsTimeRate(3);

                    fearVector = dogPos - m_Particles[i].position;

                    //Debug.DrawLine(dogPos, dogPos - fearVector, Color.black, .5f);
                    m_Particles[i].velocity -= fearVector;
                    m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
                    m_Particles[i].velocity.Normalize();
                    if (distFromObj < .2f)
                    {
                        distFromObj = .2f;
                    }
                    accel = (fearStrength / distFromObj);

                    m_Particles[i].velocity = m_Particles[i].velocity * accel;
                    m_Particles[i].velocity.Normalize();
                }
            }

            #endregion
            #region Walls
			if (obs != null)
			{
				for (int j = 0; j < obs.Count; j++)
				{
					distFromObj = Vector3.Distance(m_Particles[i].position, obs[j].transform.position);

					if (distFromObj < obs[j].radius)
					{
						prevVel = m_Particles[i].velocity.magnitude;

						obsPos = new Vector3(obs[j].transform.position.x + m_Particles[i].velocity.x * Time.deltaTime, obs[j].transform.position.y + m_Particles[i].velocity.y * Time.deltaTime, 0);
						fearVector = obsPos - m_Particles[i].position;

						m_Particles[i].velocity -= fearVector;
						m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
						m_Particles[i].velocity.Normalize();

						Debug.DrawLine(m_Particles[i].position, m_Particles[i].position + m_Particles[i].velocity.normalized, Color.green, 5.0f);
						//m_Particles[i].velocity = m_Particles[i].velocity;
						//m_Particles[i].velocity.Normalize();
					}
				}
			}
			#endregion
            if (obs != null)
            {
                for (int j = 0; j < obs.Count; j++)
                {
                    distFromObj = Vector3.Distance(m_Particles[i].position, obs[j].transform.position);

                    if (distFromObj < obs[j].radius)
                    {
                        prevVel = m_Particles[i].velocity.magnitude;

                        obsPos = new Vector3(obs[j].transform.position.x + m_Particles[i].velocity.x * Time.deltaTime, obs[j].transform.position.y + m_Particles[i].velocity.y * Time.deltaTime, 0);
                        fearVector = obsPos - m_Particles[i].position;

                        m_Particles[i].velocity -= fearVector;
                        m_Particles[i].velocity = new Vector3(m_Particles[i].velocity.x, m_Particles[i].velocity.y, 0);
                        m_Particles[i].velocity.Normalize();

                        Debug.DrawLine(m_Particles[i].position, m_Particles[i].position + m_Particles[i].velocity.normalized, Color.green, 5.0f);
                        //m_Particles[i].velocity = m_Particles[i].velocity;
                        //m_Particles[i].velocity.Normalize();
                    }
                }
            }
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
        bunnyPart.SetParticles(m_Particles, numParticlesAlive);
        yield return null;
    }
	#endregion

	#region Coloring
	void RandomParticleColor()
	{
		InitializeIfNeeded();

		int numParticlesAlive = bunnyPart.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			if (forcePalette)
			{
				bunnyColor[i] = Random.Range(0, PaletteColor.Length);
				//m_Particles[i].color = GetRandColor();
			}
			else
			{
			//m_Particles[i].color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
			}
		}

		paletteChanged = true;
		// Apply the particle changes to the particle system
		//bunnyPart.SetParticles(m_Particles, numParticlesAlive);
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

	private Color GetRandColor()
	{
		if (PaletteColor.Length > 0)
		{
			return PaletteColor[Random.Range(0, PaletteColor.Length)];
		}
		return Color.black;
	}

	public void SetPalette(Palette newPalette)
	{
		paletteChanged = true;
		for(int i = 0; i < newPalette.PaletteColor.Length; i++)
		{
			PaletteColor[i] = newPalette.PaletteColor[i];
		}
	}
	#endregion

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
