using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector3 worldPosition; //Posición en las coordenadas del mundo

    //Posición discreta del nodo basado en las coordenadas del grid. Sirve también como índice en el arreglo grid.
    public int gridX;
    public int gridY;

    // Propiedades de los recursos de la celda
    public float sugarCapacity;
    public float sugarLevel;

    public Cell(Vector2 _worldPosition, int _gridX, int _gridY, float _sugarCapacity, float _sugarLevel)
{
    worldPosition = _worldPosition;
    gridX = _gridX;
    gridY = _gridY;
    sugarCapacity = _sugarCapacity;
    sugarLevel = _sugarLevel;
}
}
