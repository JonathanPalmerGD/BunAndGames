using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	
	[System.Serializable]
	public struct VolumeList
	{
		[SerializeField]
		public int[] Volumes;
	}
	public List<AudioComponent> MusicParts;
	[SerializeField]
	public VolumeList[] Part_Volumes_By_State;
	[SerializeField]
	public string[] Part_Names;
	[SerializeField]
	public string[] State_Labels;
	private GameObject[] part_objects;
	public GameObject Music_Component_Prefab;
	public GameObject Easter_Egg_Prefab;
	
	/*DEBUG*/
	private	int state;
	public bool state_change;

	// Use this for initialization
	void Start () {
		part_objects = new GameObject [Part_Names.Length];
		for(int i = 0; i < Part_Names.Length; i++){
			GenerateComponent (Part_Names [i], i);
		}
		setInitialState ();
		playParts ();
		AddEasterEggs ();
	}

	private void AddEasterEggs(){
		GameObject obj = (GameObject)(GameObject.Instantiate (Easter_Egg_Prefab));
		obj.transform.parent = gameObject.transform;
	}

	private void playParts(){
		for(int i = 0; i<Part_Names.Length; i++) {
			part_objects[i].GetComponent<AudioComponent>().PleasePlay = true;
		}
	}

	void GenerateComponent(string _name, int index){
		GameObject obj = (GameObject)(GameObject.Instantiate (Music_Component_Prefab));
		obj.transform.parent = gameObject.transform;
		AudioComponent aud = obj.GetComponent<AudioComponent> ();
		part_objects[index] = obj;
		aud.gameObject.name = _name;
		aud.source.clip = (AudioClip)Resources.Load(_name);
		aud.source.loop = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (state_change) {
			state_change = false;
			if(state == 0){
				StartGame();
			}
			else {
				setInitialState ();
			}
		}
	}

	private void setInitialState (){
		SetState (0);
	}

	public void SetState(int _pos){
		SetState ("", _pos);
	}

	public void SetState(string _state, int _pos = -1 /* ONLY USE THIS IF YOU KNOW THE INDEX! */){
		int pos = _pos;
		if (_pos == -1) {
			pos = System.Array.IndexOf (State_Labels, _state);
		}
		else if(_pos > State_Labels.Length-1){
			pos = -1;
		}

		if (pos > -1)
		{
			SetVolumes(pos);
		}
		else{
			Debug.Log ("ERROR: STATE NOT FOUND!");
		}
		state = pos;
	}

	private void SetVolumes(int index){
		int[] volumes = Part_Volumes_By_State[index].Volumes;
		for(int i = 0; i < Part_Names.Length; i++){
			setComponentVolume(Part_Names[i],volumes[i]);
		}
	}

	// Sets the component volume for a track of the song currently being played
	private void setComponentVolume(string part_name, float volume){
		GameObject.Find(part_name).GetComponent<AudioComponent>().volume = volume;
	}

	// Resets all audio tracks for restarting of the game
	public void ResetGame(){
		foreach(GameObject g in part_objects){
			g.GetComponent<AudioSource>().Stop();
			g.GetComponent<AudioSource>().volume = 0;
			setInitialState();
		}
		playParts ();
	}

	// Starts all audio tracks
	public void StartGame(){
		SetState (1);
	}
}