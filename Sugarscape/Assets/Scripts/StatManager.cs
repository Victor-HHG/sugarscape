using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatManager : MonoBehaviour
{
    public int currentStep;
    public Text textoPasos;

    void Start(){
        currentStep = 0;
    }

    public void IncreaseStep(){
        currentStep += 1;
        textoPasos.text = "Pasos: "+currentStep;
    }
}
