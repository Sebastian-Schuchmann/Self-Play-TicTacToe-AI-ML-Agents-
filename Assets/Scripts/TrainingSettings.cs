using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class TrainingSettings : MonoBehaviour
{
    public bool TrainingMode;

    public Volume PostProcessing;
    public NNModel DefaultModel;

    public AgentManager HumanField;

    private void Start()
    {
        if (Academy.Instance.IsCommunicatorOn)
        {
            TrainingMode = true;
            SetTrainingMode();
        }
    }

    private void OnValidate()
    {
        SetTrainingMode();
    }

    private void SetTrainingMode()
    {
        if (TrainingMode)
        {
            QualitySettings.SetQualityLevel(0);
            PostProcessing.gameObject.SetActive(false);

            foreach (var behaviorParameterse in FindObjectsOfType<BehaviorParameters>())
            {
                behaviorParameterse.Model = null;
            }

            foreach (var agentManager in FindObjectsOfType<AgentManager>())
            {
                agentManager.NiceToWatchMode = false;
                agentManager.humanIsPlaying = false;
                agentManager.GetComponent<Board>().Log = false;
            }
        }
        else
        {
            QualitySettings.SetQualityLevel(2);
            PostProcessing.gameObject.SetActive(true);

            foreach (var behaviorParameterse in FindObjectsOfType<BehaviorParameters>())
            {
                behaviorParameterse.Model = DefaultModel;
            }

            foreach (var agentManager in FindObjectsOfType<AgentManager>())
            {
                agentManager.NiceToWatchMode = true;
                agentManager.GetComponent<Board>().Log = true;
            }

            HumanField.humanIsPlaying = true;
        }
    }
}
