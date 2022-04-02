using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvanceOneStep : MonoBehaviour
{
    public AgentManager agentManager;
    public StatManager statManager;
    

    public void ButtonClicked(){
        if(agentManager.AllAgentsIddle()){
            agentManager.MoveAgents();
            statManager.IncreaseStep();
        }
    }

    
}
