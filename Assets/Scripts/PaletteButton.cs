using UnityEngine;
using System.Collections;

public class PaletteButton : MonoBehaviour {
	public int PaletteIndex = 0;
	public Color PaletteColor = Color.white;
	public UnityEngine.UI.Image Img;

	// Update is called once per frame
	void Update () {
		PaletteColor = GameManager.Inst.bunPart.PaletteColor[PaletteIndex];
		Img.color = Color.Lerp(Img.color,PaletteColor,0.2f);
	}

	public void SetCurrentColorToIndex() {
		GameManager.Inst.paintingIndex = PaletteIndex;
	}
}
