using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {
    public bool Active = false;
    public Button Btn;
    public List<ToggleButton> otherBtns;

    public void Clicked() {
        foreach (ToggleButton _tb in otherBtns) {
            _tb.SetInactive();
        }
        SetActive();
    }

    public void SetActive() {
        Btn.interactable = false;     
    }

    public void SetInactive() {
        Btn.interactable = true;
    }
}