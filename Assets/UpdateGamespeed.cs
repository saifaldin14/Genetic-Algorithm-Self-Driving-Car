using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateGamespeed : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;

    void Update() {
        Time.timeScale = slider.value;
    }
}
