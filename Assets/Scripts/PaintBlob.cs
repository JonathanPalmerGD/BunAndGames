using UnityEngine;
using System.Collections;

public class PaintBlob : MonoBehaviour {
    public static Color PaintColor = Color.cyan;
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
        gameObject.renderer.material.color = PaintColor;
		scale = gameObject.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        //gameObject.renderer.material.color = Color.Lerp(gameObject.renderer.material.color, PaintColor,0.1f)
		if (state == BState.Done) {
			Destroy(gameObject);
		}
		if (state == BState.Approaching) {
			ScaleMultiplier = 1;
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, Target, 0.35f);
			if ((gameObject.transform.position - Target).magnitude == 0) { state = BState.Splat; }
		}
		if (state == BState.Splat) {
			ScaleMultiplier = 1 + (1.5f * UPPERLIMIT(SplatFrame, 2f));
			if (SplatFrame > 40) { 
				SplatFrame = 0;
				state = BState.Done;
			}

			SplatFrame++;
		}
		gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, scale * ScaleMultiplier, 0.1f);
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