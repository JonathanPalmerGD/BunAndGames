using UnityEngine;
using System.Collections.Generic;

public class EasterEggManager : MonoBehaviour {
	public int konamiTimeLimit = 0;
	public int konami_index = 0;
	public bool konami_activating = false;
	public bool konami_activated = false;
	public bool debug = true;
	public GameObject pausePrefab;
	public List<KeyCode> keys;
	
	// Use this for initialization
	void Start () {
		konami_activating = false;
		konamiTimeLimit = 0;

		keys = new List<KeyCode>();
		
		keys.Add(KeyCode.UpArrow);
		keys.Add(KeyCode.UpArrow);
		keys.Add(KeyCode.DownArrow);
		keys.Add(KeyCode.DownArrow);
		keys.Add(KeyCode.LeftArrow);
		keys.Add(KeyCode.RightArrow);
		keys.Add(KeyCode.LeftArrow);
		keys.Add(KeyCode.RightArrow);
		keys.Add(KeyCode.B);
		keys.Add(KeyCode.A);
		keys.Add(KeyCode.Return);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey ("up")){
			if(debug){Debug.Log("Up Arrow!");}
		}
		if(!konami_activating && Input.GetKey(keys[0])&& !konami_activated){
			konami_activating=true;
			konami_index = 1;
			
			if(debug){Debug.Log("Up Arrow!");}
		}
		if(konami_index > 0 && !konami_activated){
			konami_code_update();
			if(debug){Debug.Log("KONAMI_CODE_UPDATE!");}
		}
	}

	void konami_code_update(){
		if(konami_index>=keys.Count){
			konami_activated = true;
			activate_konami_code();
			konami_activating = false;
		}
		else if(isCurrentKonamiKeyDown()){
			konami_index++;
			konamiTimeLimit=200;
		}
		else if(konamiTimeLimit<=0){
			konamiTimeLimit=0;
			konami_activating=false;
			konami_index = 0;
		}
		else
			konamiTimeLimit--;
	}

	void activate_konami_code(){
		if(debug){Debug.Log("Konami Code Activated!");}
		GameObject.Instantiate(pausePrefab, Vector3.zero, Quaternion.identity);
	}

	bool isCurrentKonamiKeyDown(){
		if(Input.GetKey(keys[konami_index])){
			if(debug){Debug.Log(keys[konami_index].ToString() + " Pressed!");}
			return true;
		}
		return false;
	}
}
