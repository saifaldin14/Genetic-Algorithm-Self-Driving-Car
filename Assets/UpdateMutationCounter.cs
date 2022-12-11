using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GeneticManager;

public class UpdateMutationCounter : MonoBehaviour
{
    public UnityEngine.UI.Text mutations;

    void Update() {
        mutations.text = "Mutation #: " + GeneticManager.getMutationCounter().ToString();
    }
}
