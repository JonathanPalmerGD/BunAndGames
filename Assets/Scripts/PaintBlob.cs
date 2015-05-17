using UnityEngine;
using System.Collections;

public class PaintBlob : MonoBehaviour {
    public const float SCALE_MULT_BASE = 1.2f;
    public static Color PaintColor = Color.cyan;

    public Color BlobColor = Color.black;
	public bool Active;
	public Vector3 Target;
	public float ScaleMultiplier = 1;
	public Vector3 scale;
    private float alpha = 1.0f;

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
        if (state == BState.Done) {
            if (BlobColor.a < 0.05f)
                Destroy(gameObject);
            else {
                BlobColor = gameObject.renderer.material.GetColor("_Color");
                BlobColor.a = alpha = Mathf.Lerp(alpha, 0, 0.2f);
                gameObject.renderer.material.SetColor("_Color", BlobColor);
            }
		}
		if (state == BState.Approaching) {
			ScaleMultiplier = 1;
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, Target, 0.35f);
			if ((gameObject.transform.position - Target).magnitude < 0.05f) { state = BState.Splat; }
		}
		if (state == BState.Splat) {
			ScaleMultiplier = 1 + (SCALE_MULT_BASE * 2 * UpperLimit(3.5f*SplatFrame, 4f)/(SplatFrame-1));
			if (SplatFrame > 20) { 
				SplatFrame = 0;
				state = BState.Done;
			}
			SplatFrame++;
		}

		if(state != BState.Done){
            Debug.Log("Scale:" + scale);
            Debug.Log("ScaleM:" + ScaleMultiplier);
            Mathf.Clamp(ScaleMultiplier, 0, 10);
            Debug.Log("Wat: " + Vector3.Lerp(gameObject.transform.localScale, scale * ScaleMultiplier, 0.26f));
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, scale * ScaleMultiplier, 0.26f);
            Debug.Log(gameObject+"\n");
        }
	}

	public void Approach(Vector3 _target) {
		Target = _target;
		state = BState.Approaching;
		Active = true;
	}

	public int UpperLimit(int i, float f){
		if (i > f)
			return (int)f;
		return i;
	}

	public int UpperLimit(int i, int i2)
	{
		if (i > i2)
			return i2;
		return i;
	}

    public float UpperLimit(float f, float f2)
    {
        if (f > f2)
            return f2;
        return f;
    }

    public float UpperLimit(float f, int i)
    {
        if (f > i)
            return (float)i;
        return f;
    }
}