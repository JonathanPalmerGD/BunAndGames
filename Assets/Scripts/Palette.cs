using UnityEngine;
using System.Collections;

public class Palette
{
	//Holds 5 colors
	public Color[] PaletteColor = { new Color( .270f, .807f, .937f),
									new Color( 1.00f, .960f, .647f),
									new Color( 1.00f, .831f, .854f),
									new Color( .600f, .823f, .894f),
									new Color( .847f, .792f, .705f), };

	public Palette(Color first, Color second, Color third, Color fourth, Color fifth)
	{
		PaletteColor = new Color[] { first, second, third, fourth, fifth };
	}
}
