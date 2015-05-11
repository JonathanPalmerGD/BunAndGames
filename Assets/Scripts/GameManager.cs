using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject bunEmitterPrefab;
	public GameObject dogPrefab;
	public GameObject bunEmitter;
	public GameObject dog;
	public Canvas menuUI;
	public Canvas barkCanvas;
	public Slider bunnyCount;
	public AudioManager audioMan;
	public static GameManager Inst;
	public float playerPoints = 0;
	public BunnyParticles bunPart;
	public DogController dogCon;

	public enum InputMode { Dog, Painting, Shop, Veggies, Environment };
	public InputMode mode = InputMode.Dog;

	public Text pointDisplay;

	public void Start()
	{
		Inst = this;
		barkCanvas.gameObject.SetActive(false);
	}

	public void Update()
	{
		if (pointDisplay != null)
		{
			pointDisplay.text = ((int)playerPoints).ToString();
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			ChangeInputMode(InputMode.Dog);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			ChangeInputMode(InputMode.Painting);
		}
	}

	public void ChangeInputMode(InputMode targetMode)
	{
		Debug.Log("[GameManager]\tChanging mode from " + mode.ToString() + " to " + targetMode.ToString());
		mode = targetMode;
	}

	public void GainPointsTimeRate(float amount)
	{
		playerPoints += amount * Time.deltaTime;
	}

	public void GainPoints(float amount)
	{
		playerPoints += amount;
	}

	public void BeginGame()
	{
		audioMan.state_change = true;

		dog.SetActive(true);
		bunEmitter.SetActive(true);
		bunPart = bunEmitter.GetComponent<BunnyParticles>();
		bunPart.HowManyBunnies = (int)bunnyCount.value;

		//GameObject dog = (GameObject)GameObject.Instantiate(dogPrefab, Vector3.zero, Quaternion.identity);
		//BunnyParticles bunPart = ((GameObject)GameObject.Instantiate(bunEmitterPrefab, Vector3.zero, Quaternion.identity)).GetComponent<BunnyParticles>();
		bunPart.dog = dog;
		menuUI.gameObject.SetActive(false);

		//#if UNITY_ANDROID
		barkCanvas.gameObject.SetActive(true);
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
		menuUI.gameObject.SetActive(true);

//#if UNITY_ANDROID
		barkCanvas.gameObject.SetActive(false);
//#endif
	}
}
