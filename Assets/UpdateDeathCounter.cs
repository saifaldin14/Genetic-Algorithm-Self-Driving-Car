using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CarController;

public class UpdateDeathCounter : MonoBehaviour
{   

    public UnityEngine.UI.Text deaths;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deaths.text = "Deaths #: " + CarController.getDeathCounter().ToString();
    }
}
