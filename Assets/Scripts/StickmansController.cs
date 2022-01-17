using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StickmansController : MonoBehaviour
{
    private static StickmansController _instance;
    public static StickmansController Instance => _instance;

    public NavMeshAgent agentPrefab;
    public GameObject agentsContainer;
    public GameController gameController;

    private List<NavMeshAgent> stickmen;
    private List<Vector3> points;
    List<Vector3> availablePoints;
    private NavMeshPath path;
    private Vector3 startPosition;
    private float pathEndThreshold;
    private bool isSphereCompleted;
    float allowableAccuracy;
    private int totalPoints;
    private int agentsCount;

    private void Start()
    {
        _instance = this;
        agentsCount = 0;
        allowableAccuracy = 0.95f;
        isSphereCompleted = false;
        path = new NavMeshPath();
        startPosition = new Vector3(5, 0, 5);
        pathEndThreshold = 0.1f;
        stickmen = new List<NavMeshAgent>();
        stickmen.Add(CreateNewAgent(startPosition));
        GenerateDestinationPoints();
        BuildAvailablePaths();
    }
    private void FixedUpdate()
    {
        if (isSphereCompleted)
        {
            PathCheckEnd();
            Debug.Log(stickmen.Count + " <= " + totalPoints + " * " + (1 - allowableAccuracy));
            if (stickmen.Count <= totalPoints * (1 - allowableAccuracy))
            {
                if (totalPoints * allowableAccuracy <= agentsCount)
                    gameController.ShowEndScreen(true);
                else
                    gameController.ShowEndScreen(false);
            }
        }
    }
    public void StartCheckingPaths()
    {
        isSphereCompleted = true;
    }
    private void GenerateDestinationPoints()
    {
        float mapSize = 27;
        float pointY = 0;

        points = new List<Vector3>();

        for (float currentZ = 2; currentZ < mapSize - 2; currentZ += 2)
        {
            for (float currentX = 2; currentX < mapSize - 2; currentX += 2)
            {
                points.Add(new Vector3(currentX, pointY, currentZ));
            }
        }
        totalPoints = points.Count;
    }
    public void BuildAvailablePaths()
    {
        availablePoints = new List<Vector3>();
        List<Vector3> tempPoints = new List<Vector3>(points);

        foreach (var point in tempPoints)
        {
            stickmen[0].CalculatePath(point, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                availablePoints.Add(point);
                points.Remove(point);
            }
        }

        List<NavMeshAgent> newAgents = new List<NavMeshAgent>();
        
        int maxPoints = availablePoints.Count;
        if (stickmen.Count == 1)
        {
            maxPoints--;
            newAgents.AddRange(stickmen);
        }

        for (int i = 0; i < maxPoints;)
        {
            for(int j = stickmen.Count - 1; j >= 0; j--)
            {
                newAgents.Add(CreateNewAgent(stickmen[j].nextPosition));
                i++;
                if (i >= maxPoints)
                    break;
            }
        }
        agentsCount += newAgents.Count;
        SetDestinationToNewAgents(availablePoints, newAgents);
        stickmen.AddRange(newAgents);
    }
    public void DelayBeforeBuildPaths()
    {
        Invoke("BuildAvailablePaths", 0.5f);
    }
    private NavMeshAgent CreateNewAgent(Vector3 pos)
    {
        NavMeshAgent newAgent = Instantiate(agentPrefab, pos, Quaternion.identity);
        newAgent.transform.parent = agentsContainer.transform;
        return newAgent;
    }
    private void SetDestinationToNewAgents(List<Vector3> points, List<NavMeshAgent> agents)
    {
        for(int i = 0; i < agents.Count; i++)
            agents[i].SetDestination(points[i]);
    }
    private void PathCheckEnd()
    {
        List<NavMeshAgent> lastAgents = new List<NavMeshAgent>(stickmen);
        foreach(var agent in lastAgents)
        {
            Debug.Log(agent.hasPath + " - " + agent.remainingDistance + "  " + agent.stoppingDistance + " + " + pathEndThreshold);
            if (agent.remainingDistance <= agent.stoppingDistance + pathEndThreshold)
            {
                stickmen.Remove(agent);
            }
        }
    }
}
