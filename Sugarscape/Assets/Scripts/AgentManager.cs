using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    GameObject[] agents;

    public void MoveAgents(){
        agents = GameObject.FindGameObjectsWithTag("Agent");
        foreach(GameObject agentGO in agents){
            agentGO.GetComponent<Agent>().LookAndMove();
        }
    }

    public bool AllAgentsIddle(){
        agents = GameObject.FindGameObjectsWithTag("Agent");
        bool allIdle = true;
        foreach(GameObject agentGO in agents){
            if(!agentGO.GetComponent<Agent>().iddle){
                allIdle = false;
                break;
            }
        }
        return(allIdle);
    }
}
