using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

public enum HeuristicMethod
{
    Random,
    MinMax
}

public class TicTacToeAgent : Agent
{
    public Player type;
    [FormerlySerializedAs("boardState")] public Board board;
    public HeuristicMethod heuristicMethod;

    public override void OnEpisodeBegin()
    {
        type = GetComponent<BehaviorParameters>().TeamId == 0 ? Player.X : Player.O;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var availableOptions = (int[]) board.GetAvailableFields();

        if (heuristicMethod == HeuristicMethod.Random)
        {
            int randomField = availableOptions[Random.Range(0, availableOptions.Length)];
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = randomField;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var field in board.Fields)
        {
            sensor.AddOneHotObservation(field.ObserveField(type), 3);
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        board.SelectField(Mathf.FloorToInt(vectorAction[0]), type);
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        actionMasker.SetMask(0, board.GetOccupiedFields());
    }
}