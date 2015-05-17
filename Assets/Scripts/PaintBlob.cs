using UnityEngine;
using System.Collections;

public class PaintBlob : MonoBehaviour {
	public bool Active;
	public Vector3 Target;
	public float ScaleMultiplier = 1;
	public Vector3 scale;

	public int SplatFrame = 0;

	public enum BState {
		Init,
		Approaching,
		Splat,
		Done
	}

	private BState state = BState.Init;

	// Use this for initialization
	void Start () {
		scale = gameObject.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == BState.Done) {
			Destroy(gameObject);
		}
		if (state == BState.Approaching) {
			ScaleMultiplier = 1;
            Debug.Log("HI!!")
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, Target, 0.05f);
            if ((gameObject.transform.position - Target).magnitude < 0.05f) { state = BState.Splat; }
		}
		if (state == BState.Splat) {
			ScaleMultiplier = 1 + (0.5f * UPPERLIMIT(SplatFrame, 2f)/(40.1f-SplatFrame));
            if (SplatFrame > 40) { 
                SplatFrame = 0;
                state = BState.Done;
            }

            SplatFrame++;
		}
		gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, scale * ScaleMultiplier, 0.01f);
	}

    public void Approach(Vector3 _target) {
        Target = _target;
        state = BState.Approaching;
        Active = true;
    }

	public int UPPERLIMIT(int i, float f){
		if (i > f)
			return (int)f;
        return i;
	}

    public int UPPERLIMIT(int i, int i2)
    {
        if (i > i2)
            return i2;
        return i;
    }
}