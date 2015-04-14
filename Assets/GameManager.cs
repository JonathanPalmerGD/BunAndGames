using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject bunEmitterPrefab;
	public GameObject dogPrefab;
	public Canvas menuUI;
	public AudioManager audioMan;

	public void BeginGame()
	{

		audioMan.state_change = true;

		GameObject dog = (GameObject)GameObject.Instantiate(dogPrefab, Vector3.zero, Quaternion.identity);
		BunnyParticles bunPart = ((GameObject)GameObject.Instantiate(bunEmitterPrefab, Vector3.zero, Quaternion.identity)).GetComponent<BunnyParticles>();

		bunPart.dog = dog;
		menuUI.gameObject.SetActive(false);
	}
}
