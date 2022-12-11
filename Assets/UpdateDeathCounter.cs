using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CarController;

public class UpdateDeathCounter : MonoBehaviour
{
    public UnityEngine.UI.Text deaths;

    void Update() {
        deaths.text = "Deaths #: " + CarController.getDeathCounter().ToString();
    }
}

