using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour 
{
	public static Shop Inst;
    public GameObject PaintBtn;
	public int paintingCost = 200;
	public int paletteCost = 150;
	public int environmentCost = 750;
	public int veggieCost = 750;

	public void Awake()
	{
		Inst = this;
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GameManager.Inst.CloseShop();
		}
        if (Input.GetMouseButtonUp(0))
            GameManager.Inst.CloseShop();
	}

	public void PurchasePainting()
	{
		if (!GameManager.Inst.unlockedPainting && GameManager.Inst.playerPoints >= paintingCost)
		{
			Debug.Log("Purchased Painting Mode\n");
            PaintBtn.SetActive(true);
			GameManager.Inst.playerPoints -= paintingCost;

			GameManager.Inst.unlockedPainting = true;
		
			//Overlay the 'Purchased' option
			//Disable the button.
		}
	}
}
