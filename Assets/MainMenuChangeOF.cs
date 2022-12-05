using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CarController;

public class MainMenuChangeOF : MonoBehaviour
{
    public Text textBox;

    public void HandleInputData(int val){

        if (val == 0){
            textBox.text = "Selected: De Jong";
            CarController.objectiveFunction = "De Jong";

        }
        if (val == 1){
            textBox.text = "Selected: Rosenbrock";
            CarController.objectiveFunction = "Rosenbrock";

        }
        if (val == 2){
            textBox.text = "Selected: Himmelblau";
            CarController.objectiveFunction = "Himmelblau";

        }
    }
}
