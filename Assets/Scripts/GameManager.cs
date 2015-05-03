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

	public void Start()
	{
		barkCanvas.gameObject.SetActive(false);
	}

	public void BeginGame()
	{
		audioMan.state_change = true;

		dog.SetActive(true);
		bunEmitter.SetActive(true);
		BunnyParticles bunPart = bunEmitter.GetComponent<BunnyParticles>();
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
