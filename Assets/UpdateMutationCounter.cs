using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GeneticManager;

public class UpdateMutationCounter : MonoBehaviour
{   
    public UnityEngine.UI.Text mutations;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mutations.text = "Mutation #: " + GeneticManager.getMutationCounter().ToString();
    }
}
