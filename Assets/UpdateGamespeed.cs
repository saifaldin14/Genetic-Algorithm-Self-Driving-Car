using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateGamespeed : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = slider.value;
    }
}
