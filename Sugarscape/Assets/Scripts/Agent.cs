using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //Genes. Se asignan al nacer
    public float sugarMetabolism;
    public int visionLevel;

    //Otras
    
    public float sugarEndowment;


    public Cell agentCell;
    GameObject agentObject;
    public GroundManager ground;
    float moveSpeed;
    public float moveTime;
    bool moving = false;
    public bool iddle = true;
    Vector2 moveTarget;
    Vector2 movePrimTarget;
    Vector2 moveSecTarget;
    public bool targetGizmos;

    void Start(){
        agentCell = ground.CellFromWorldPosition(transform.position);
    }
    
    void FixedUpdate(){

        if(moving){
            Move(moveTarget);
        }
        
    }

    public void LookAndMove(){
        iddle = false;
        movePrimTarget = SetNewPosition();
        CheckTeleport();
        UpdateSpeed();
        moving = true;
        Move(moveTarget);
    }

    void Move(Vector2 target){
        Vector2 currentPosition = transform.position;
        if(target != currentPosition){            
            Vector2 direction = Vector2.MoveTowards(currentPosition, target, moveSpeed*Time.deltaTime);
            transform.position = direction;
        }
        else{
            moving = false;
            iddle = true;
        }
        TorusMove();        
    }

    void TorusMove(){
        // Código para transportar al agente cuando camina hacia el borde del mapa.
        if(transform.position.x > ground.xBorderPos){
            transform.position = new Vector2(ground.xBorderNeg, transform.position.y);
            moveTarget = movePrimTarget;
        }
        if(transform.position.x < ground.xBorderNeg){
            transform.position = new Vector2(ground.xBorderPos, transform.position.y);
            moveTarget = movePrimTarget;
        }
        if(transform.position.y > ground.yBorderPos){
            transform.position = new Vector2(transform.position.x, ground.yBorderNeg);
            moveTarget = movePrimTarget;
        }
        if(transform.position.y < ground.yBorderNeg){
            transform.position = new Vector2(transform.position.x, ground.yBorderPos);
            moveTarget = movePrimTarget;
        }
    }

    Vector2 SetNewPosition(){
        Cell[] posibleCells = Vision();
        int selected = Random.Range(1, posibleCells.Length);
        agentCell = posibleCells[selected];
        return(agentCell.worldPosition);
    }

    void CheckTeleport(){
        if (Vector2.Distance(transform.position, movePrimTarget) > visionLevel){
            if(transform.position.y == movePrimTarget.y){
                if (transform.position.x > movePrimTarget.x){
                moveSecTarget = new Vector2(transform.position.x + visionLevel, transform.position.y);
                }
                if (transform.position.x < movePrimTarget.x){
                moveSecTarget = new Vector2(transform.position.x - visionLevel, transform.position.y);
                }
            }
            if(transform.position.x == movePrimTarget.x){
                if (transform.position.y > movePrimTarget.y){
                moveSecTarget = new Vector2(transform.position.x, transform.position.y + visionLevel);
                }
                if (transform.position.y < movePrimTarget.y){
                moveSecTarget = new Vector2(transform.position.x, transform.position.y - visionLevel);
                }
            }
            // ¿Debería tener una condición en caso de que ninguno de los dos ejes esté alineado?
            moveTarget = moveSecTarget;
        }
        else{
            moveTarget = movePrimTarget;
        }
    }

    Cell[] Vision(){
    int x = agentCell.gridX;
    int y = agentCell.gridY;
    // 4 por el rango de visión por las cuatro direcciones en las que puede ver
    int cellNumber = visionLevel * 4 + 1;
    Cell[] cellsInRange = new Cell[cellNumber];
    cellsInRange[0] = agentCell;
    for(int i = 1; i<= visionLevel; i++){
        // Ver a la derecha
        cellsInRange[4*(i-1)+ 1] = ground.grid[(x + i) % ground.gridSizeX, y];
        // Ver abajo
        cellsInRange[4*(i-1)+ 2] = ground.grid[x, (y - i + ground.gridSizeY) % ground.gridSizeY];
        // Ver a la izquierda
        cellsInRange[4*(i-1)+ 3] = ground.grid[(x - i + ground.gridSizeX) % ground.gridSizeX, y];
        // Ver arriba
        cellsInRange[4*(i-1)+ 4] = ground.grid[x, (y + i) % ground.gridSizeY];
        
    }
    return(cellsInRange);
}

    void OnDrawGizmosSelected(){
        if (moveTarget != null & targetGizmos){
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, moveTarget);
        }
    }

    void UpdateSpeed(){
        if(moveTarget != movePrimTarget){
            float distanceDirect = Vector2.Distance(transform.position, movePrimTarget);
            float distance = ground.gridWorldSize.x - distanceDirect;               // OJO Aquí asumimos que el mundo es cuadrado.
            moveSpeed = distance / moveTime;
        }
        else{
            float distance = Vector2.Distance(transform.position, movePrimTarget);
            moveSpeed = distance / moveTime;
        }
    }

}
