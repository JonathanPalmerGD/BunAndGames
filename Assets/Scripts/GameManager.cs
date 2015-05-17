using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
	#region Dog, BunEmtiter DogController & Particles
	public GameObject bunEmitterPrefab;
	public GameObject dogPrefab;
	public GameObject bunEmitter;
	public GameObject dog;
	public BunnyParticles bunPart;
	public DogController dogCon;
	#endregion

	#region Self-Instance, Canvas, & Overhead
	public static GameManager Inst;
	public AudioManager audioMan;
	public Canvas shopCanvas;
	public Canvas menuCanvas;
	public Canvas inGameCanvas;
	public Slider bunnyCount;
	#endregion

	public float playerPoints = 0;
	public List<Palette> palettes;

	public int paintingIndex = 0;

	#region Unlocked Abilities
	public bool unlockedPainting;
	public bool unlockedVeggies;
	public bool unlockedEnvironment;
	#endregion

	public enum InputMode { Dog = 0, Painting, Shop, Veggies, Environment };
	public InputMode mode = InputMode.Dog;
	public InputMode previousMode = InputMode.Dog;

	public Text pointDisplay;

	public void Start()
	{
		Inst = this;
		shopCanvas.gameObject.SetActive(false);
		inGameCanvas.gameObject.SetActive(false);

		palettes = new List<Palette>();

		Palette pal = new Palette(new Color(.270f, .807f, .937f),
									new Color(1.00f, .960f, .647f),
									new Color(1.00f, .831f, .854f),
									new Color(.600f, .823f, .894f),
									new Color(.847f, .792f, .705f));
		palettes.Add(pal);
		pal = new Palette(Color.red, Color.blue, Color.white, Color.white, Color.white);

		palettes.Add(pal);
		pal = new Palette(Color.red, Color.gray, Color.cyan, Color.green, Color.yellow);
		palettes.Add(pal);
		pal = new Palette(new Color(.270f, .807f, .937f),
									new Color(1.00f, .960f, .647f),
									new Color(1.00f, .831f, .854f),
									new Color(.600f, .823f, .894f),
									new Color(.847f, .792f, .705f));
		//palettes.Add(pal);
		Debug.Log(palettes.Count + "\n");
	}

	public void Update()
	{
		if (pointDisplay != null)
		{
			pointDisplay.text = ((int)playerPoints).ToString();
		}

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.S))
		{
			GainPoints(3000);
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			ChangeInputMode(InputMode.Dog);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			ChangeInputMode(InputMode.Painting);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			ChangeInputMode(InputMode.Shop);
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			SetPalette(0);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			SetPalette(1);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			SetPalette(2);
		}
	}

	#region State Machine Control
	public void ChangeInputMode(int targetMode)
	{
		ChangeInputMode((InputMode)targetMode);
	}
	
	public void ChangeInputMode(InputMode targetMode)
	{
		Debug.Log("[GameManager]\tChanging mode from " + mode.ToString() + " to " + targetMode.ToString());
		previousMode = mode;
		mode = targetMode;
	}

	public void OpenShop()
	{
		if (mode != InputMode.Shop)
		{
			ChangeInputMode(InputMode.Shop);
			shopCanvas.gameObject.SetActive(true);
		}
	}
	public void CloseShop()
	{
		if (mode == InputMode.Shop)
		{
			ChangeInputMode(previousMode);
			shopCanvas.gameObject.SetActive(false);
		}
	}
	#endregion

	#region Point Gain
	public void GainPointsTimeRate(float amount)
	{
		playerPoints += amount * Time.deltaTime;
	}

	public void GainPoints(float amount)
	{
		playerPoints += amount;
	}
	#endregion

	#region Button-called Methods
	public void BeginGame()
	{
		audioMan.state_change = true;

		dog.SetActive(true);
		bunEmitter.SetActive(true);
		shopCanvas.gameObject.SetActive(false);
		bunPart = bunEmitter.GetComponent<BunnyParticles>();
		bunPart.HowManyBunnies = (int)bunnyCount.value;

		//GameObject dog = (GameObject)GameObject.Instantiate(dogPrefab, Vector3.zero, Quaternion.identity);
		//BunnyParticles bunPart = ((GameObject)GameObject.Instantiate(bunEmitterPrefab, Vector3.zero, Quaternion.identity)).GetComponent<BunnyParticles>();
		bunPart.dog = dog;
		menuCanvas.gameObject.SetActive(false);

		//#if UNITY_ANDROID
		inGameCanvas.gameObject.SetActive(true);
		//#endif

		bunPart.Init();
	}	
	
	public void StopGame()
	{
		audioMan.state_change = false;

		dog.SetActive(false);
		bunEmitter.SetActive(false);
		BunnyParticles bunPart = bunEmitter.GetComponent<BunnyParticles>();
		bunPart.ClearBunnies();

		bunPart.dog = dog;
		menuCanvas.gameObject.SetActive(true);

//#if UNITY_ANDROID
		inGameCanvas.gameObject.SetActive(false);
//#endif
	}
	#endregion

	public void SetPalette(int targPaletteIndex)
	{
		bunPart.SetPalette(palettes[targPaletteIndex]);
	}

}
