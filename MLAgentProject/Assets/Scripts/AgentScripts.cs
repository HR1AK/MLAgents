using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentScripts: Agent
{
    [SerializeField] private Transform Goal;
    //[SerializeField] private Transform Wall;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floarMeshRender;

    //bool oneBonusonEpisode = false;

    public override void OnEpisodeBegin()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localPosition = new Vector3(5.5f, 0.7f, 1f);
        Goal.localPosition = new Vector3(Random.Range(-9f, 13f), -1.861768f, Random.Range(-9f, -3f));
        //oneBonusonEpisode = false;
    }
    //Obseravtion (наблюдение)
    public override void CollectObservations(VectorSensor sensor)
    {
        //позиции игрока и цели имеют по 3 координаты (x,y,z) => Space size = 6, столько параметров на вход
        //позиция игрока
        sensor.AddObservation(transform.localPosition);
        //позиция цели
        sensor.AddObservation(Goal.localPosition);
        //sensor.AddObservation(Wall.localPosition);
    }

    //Action (действие)
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void Update()
    {
         if(transform.position.y <= -2.5f)
        {
            SetReward(-0.5f);
            floarMeshRender.material = loseMaterial;
            EndEpisode();
        }  

        //if (oneBonusonEpisode == false && transform.localPosition.z <= -2f)
       // {
        //    AddReward(+0.1f);
           // oneBonusonEpisode = true;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1f);
            floarMeshRender.material = winMaterial;
            EndEpisode();
        }

        if(other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-0.5f);
            floarMeshRender.material = loseMaterial;
            EndEpisode();
        } 
    }
}
