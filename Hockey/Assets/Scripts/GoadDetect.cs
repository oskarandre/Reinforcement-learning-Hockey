using UnityEngine;

public class GoalDetect : MonoBehaviour
{
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization.
    /// Don't need to manually set.
    /// </summary>
    [HideInInspector]

    //[SerializeField] private Transform puck;
    // public AgentMove Agent;  //

    [SerializeField] private AgentMove Agent;
    [SerializeField] private Transform puck;



    public void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collision detected");

        // Touched goal.
        if (col.TryGetComponent <Wall> (out Wall Wall))
        {
            Debug.Log("Goal detected");
            Agent.ScoredAGoal();
        }
    }
}