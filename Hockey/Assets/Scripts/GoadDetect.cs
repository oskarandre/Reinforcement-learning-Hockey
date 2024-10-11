// using UnityEngine;
// using Unity.MLAgents;
// using Unity.MLAgents.Sensors;

// public class GoalDetect : MonoBehaviour
// {
//     /// <summary>
//     /// The associated agent.
//     /// This will be set by the agent script on Initialization.
//     /// Don't need to manually set.
//     /// </summary>
//     [HideInInspector]

//     //[SerializeField] private Transform puck;
//     public AgentMove Agent;  //

//     //[SerializeField] private AgentMove Agent;
//     //[SerializeField] private Transform puck;



//     public void OnTriggerEnter(Collider col)
//     {
//         Debug.Log("Collision detected");

//         // Touched goal.
//         if (col.TryGetComponent <Wall> (out Wall Wall))
//         {
//             Debug.Log("Goal detected");
//             Agent.ScoredAGoal();
//         }
//     }

// }


//Detect when the orange block has touched the goal.
//Detect when the orange block has touched an obstacle.
//Put this script onto the orange block. There's nothing you need to set in the editor.
//Make sure the goal is tagged with "goal" in the editor.

using UnityEngine;

public class GoalDetectWithInput : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]
    public AgentMove agent;  //

    void OnCollisionEnter(Collision col)
    {
        // Touched goal.
        if (col.gameObject.CompareTag("OpponentGoal"))
        {
            agent.ScoredAGoal(reward: 1f);
        }
        else if (col.gameObject.CompareTag("OwnGoal"))
        {
            agent.ScoredAGoal(reward: -1f);
        }
    }
}