using UnityEngine;
using System.Collections;

public class KillWhenSoundEnds : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		if(!(audio.timeSamples<audio.clip.samples-1)){
			Destroy(gameObject);
		}
	}
}
