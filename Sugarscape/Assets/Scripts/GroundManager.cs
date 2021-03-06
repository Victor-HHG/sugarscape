using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundManager : MonoBehaviour
{
    
    public Vector2 gridWorldSize; //Se introducen los valores en el editor.
    public float cellSize; //Se introduce en el editor.
    public bool displayGridGizmos; //Opción para graficar o no el gizmos
    public GameObject peak1Reference;
    public GameObject peak2Reference;
    public float resourceRadius;
    public int maxResources;
    public Tilemap tilescape;
    public TileBase[] sugarTiles;

    Vector2 gridCenter; 
    public Cell[,] grid;

    public int gridSizeX; //Nos dice el numero de cuadros que caben en el eje X
    public int gridSizeY; //Nos dice el número de cuadros que caben en el eje Z. 
    int maxSize;
    
    //bordes del mapa
    public float xBorderNeg, xBorderPos, yBorderNeg, yBorderPos;
        
    private void Awake()
    {
        // Se calcula el número de cuadros que caben en el grid, dividiendo
        // lo que mide el grid entre el tamaño de las celdas (o lado del cuadrito).
        // Se redondea el resultado a números enteros.
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / cellSize);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / cellSize);
        maxSize = gridSizeX * gridSizeY;

        gridCenter = transform.position;

        xBorderNeg = gridCenter.x - gridWorldSize.x/2;
        xBorderPos = gridCenter.x + gridWorldSize.x/2;
        yBorderNeg = gridCenter.y - gridWorldSize.y/2;
        yBorderPos = gridCenter.y + gridWorldSize.y/2;

        CreateGrid();
        InitiateResources();
    }

    void Start(){
        CreateTiles();
        UnlockTilesColors();
        UpdateTiles();
    }


    //Este método crea el grid a partir de las medidas y número de nodos estimados previamente en el Awake.
    void CreateGrid()
    {
        grid = new Cell[gridSizeX, gridSizeY];
        //Se determina la posición del punto inferior izquierdo. A partir de ahí se empiezan a dibujar los cuadritos.
        //Se parte de que este script está en el centro del grid. Se le resta la mitad del tamaño en x y y.
        Vector2 worldBottomLeft = gridCenter - new Vector2(gridWorldSize.x / 2, gridWorldSize.y / 2);

        //Se crea un loop doble para iterar en todas las posiciones de los cuadritos en el eje x y el eje y.
        for(int x=0; x < gridSizeX; x++)
        { 
            for (int y = 0; y < gridSizeY; y++)
            {
               //Se determina el centro de cada cuadrito, partiendo del worldBottomLeft y moviendose el diametro en cada iteración hacia x o y.
               //Además, se suma el radio para asegurar que estamos en el centro y no en una orilla.
                Vector2 worldPoint = worldBottomLeft + new Vector2(x * cellSize + cellSize/2, y * cellSize + cellSize/2);
                
                grid[x, y] = new Cell(worldPoint, x, y, 1, 1);

                //Se crea el Tile correspondiente a la celda

            }
        }
    } 

    // Este método inicializa la asignación de recursos en el grid. Sólo se ejecuta al principio.
    void InitiateResources(){
        // Se obtienen las posiciones de los dos peak
        Vector2 peak1Position = peak1Reference.transform.position;
        Vector2 peak2Position = peak2Reference.transform.position;
        
        foreach(Cell c in grid){
            //Se calcula la distancia de cada celda al peak más cercano
            float distanceToPeak1 = Vector2.Distance(peak1Position, c.worldPosition);
            float distanceToPeak2 = Vector2.Distance(peak2Position, c.worldPosition);
            float distanceNearest = Mathf.Min(distanceToPeak1, distanceToPeak2);
            // Se generan discos basados en la distancia, dividiéndola por el radio del círculo deseado.
            // Se resta dicho valor redondeado al máximo de azúcar permitido.
            int sugarCap = maxResources - Mathf.FloorToInt(distanceNearest / resourceRadius);
            // Se asegura que no haya capacidades negativas
            sugarCap = Mathf.Max(0, sugarCap);
            //Se asigna la capacidad y nivel a la celda en cuestión.
            c.sugarCapacity = sugarCap;
            c.sugarLevel = sugarCap;
        }
    }

    private void CreateTiles(){
        Vector3Int[] positionArray = new Vector3Int[maxSize];
        TileBase[] tileArray = new TileBase[maxSize];
        int counter = 0;

        foreach(Cell c in grid){
            positionArray[counter] = tilescape.WorldToCell(c.worldPosition);
            tileArray[counter] = sugarTiles[0];
            counter += 1;
        }
        
        tilescape.SetTiles(positionArray, tileArray);
    }

    private void UpdateTiles(){
        Vector3Int cellPosition;
        Color cellColor;

        foreach(Cell c in grid){
            cellPosition = tilescape.WorldToCell(c.worldPosition);
            cellColor = SugarTileColor(c.sugarLevel / maxResources);
            tilescape.SetColor(cellPosition, cellColor);
        }
    }



    //Este método dibuja líneas guía para que sólo se ven durante el desarrollo. Al parecer eso son los Gizmos.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gridCenter, gridWorldSize);

             //Se dibujan cuadritos representando cada nodo del grid.
             // Sólo se dibuja si el grid existe y la opción displayGridGizmos está activa
            if (grid != null && displayGridGizmos)
            {
                foreach (Cell c in grid)
                {
                    //Se determina el color del cuadro
                    Gizmos.color = new Color(0,1,0,c.sugarLevel / maxResources);
                    //Se dibuja el cuadro en la posición de cada cuadtrito, con cada lado igual a 1 menos un pequeño espacio entre cubos.
                    Gizmos.DrawCube(c.worldPosition, Vector2.one * (cellSize - 0.1f));
                }
            }
    }

    //Este método encuentra la celda en el que se encuentra un objeto. 
    public Cell CellFromWorldPosition(Vector2 worldPosition)
    {
        //Se determina la posición a partir de la posición relativa del objeto en el plano.
        float percentX = (worldPosition.x / gridWorldSize.x) + 0.5f; //Se suma el 0.5 porque el centro (0,0,0) está a la mitad del plano
        float percentY = (worldPosition.y / gridWorldSize.y) + 0.5f;

        //Se asegura que los porcentajes no se salgan del intervalo [0,1]
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Se determinan los índices del cuadro donde está el objeto, multiplicando el porcentaje por el número de cuadritos.
        int x = Mathf.FloorToInt(gridSizeX * percentX);
        int y = Mathf.FloorToInt(gridSizeY * percentY);

        return grid[x, y];
    }


    // Función para calcular el color de las celdas basado en la capacidad de azúcar.
    Color SugarTileColor(float x){
        Vector3 colorMax = new Vector3(0.52f, 0.6f, 0f);
        Vector3 colorMin = new Vector3(1f, 1f, 1f);
        Vector3 newColor = x * colorMax + (1-x) * colorMin;

        return(new Color(newColor.x, newColor.y, newColor.z, 1f));
    }

    void UnlockTilesColors(){
        Vector3Int cellPosition;
        
        foreach(Cell c in grid){
            cellPosition = tilescape.WorldToCell(c.worldPosition);
            tilescape.RemoveTileFlags(cellPosition, TileFlags.LockColor);
        }
    }



    
}
