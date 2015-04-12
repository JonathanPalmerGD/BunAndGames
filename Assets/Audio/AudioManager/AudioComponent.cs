using UnityEngine;
using System.Collections;

public class AudioComponent : MonoBehaviour {
	public AudioSource source;
	public bool PleasePlay = false;
	public float volume = 0;
	public float CHANGE = 0.5f;
	void Start(){
		source = gameObject.GetComponent<AudioSource>();
		source.volume = 0;
	}

	void Update(){
		if (PleasePlay) {
			PleasePlay = false;
			source.Play();
		}

		if(source.volume!=volume){
			if(Mathf.Abs(source.volume-volume)<CHANGE)
				return;
			source.volume = Mathf.Lerp(source.volume, volume, CHANGE);
		}
	}
}
