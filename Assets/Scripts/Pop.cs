using UnityEngine;
using System.Collections;

public class Pop : MonoBehaviour {
    public Color color = Color.white;
    public float alpha = 1.0f;
	
	// Update is called once per frame
	void Update () {
        if (color.a < 0.05f)
        {
            Destroy(gameObject);
        }
        else
        {
            color = gameObject.renderer.material.GetColor("_Color");
            color.a = alpha = Mathf.Lerp(alpha, 0, 0.2f);
            gameObject.renderer.material.SetColor("_Color", color);
            gameObject.transform.localScale *= 1.2f;
        }
	}
}
