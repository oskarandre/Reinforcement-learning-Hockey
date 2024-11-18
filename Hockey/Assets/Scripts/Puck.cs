using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{

    [SerializeField] private Transform OwnGoal;
    [SerializeField] private Transform OpponentGoal;

    public AgentMove agent; 

    private void OnTriggerEnter(Collider other){

         if(other.CompareTag("Stick")){
             agent.AgentReward(0.1f, "Stick");
         }
    }
}
