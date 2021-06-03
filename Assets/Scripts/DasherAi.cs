using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class DasherAi : Agent
{
    private GuidedMissileController _guidedMissileController;

    public Transform target;
    public bool trainingMode = true;

    private float _distance;
    private float _previousDistance;
    
    private float _angle;
    private float _previousAngle;

    public override void Initialize()
    {
        _guidedMissileController = GetComponent<GuidedMissileController>();

        _guidedMissileController.OnDetonation += other =>
        {
            if (!trainingMode)
            {
                Destroy(gameObject);
            }
        };
    }

    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {
            Restart();
        }
    }


    private void Update()
    {
        Debug.DrawLine(transform.position, target.position, Color.green);
    }

    /// <summary>
    /// Called when an action is received from player input or the neural network
    /// vectorAction[i] represents:
    ///Index 0: steer vector left or right
    ///Index 1: steer vector up or down
    /// </summary>
    /// <param name="actions">The actions to take</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
//        float power = Mathf.Clamp(actions.ContinuousActions[0], 0, 1f);
//        
//        _guidedMissileController.Launch(power);
//        
//        float x = Mathf.Clamp(actions.ContinuousActions[1], - 1f, 1f);
//        float y = Mathf.Clamp(actions.ContinuousActions[2], - 1f, 1f);
        
//        float x = 0;
//        float y = 0;
//        
//        switch (actions.DiscreteActions[0])
//        {
//            case 0:
//                x = -1;
//                break;
//            case 1:
//                x = 1;
//                break;
//        }
//        
//        switch (actions.DiscreteActions[1])
//        {
//            case 0:
//                y = -1;
//                break;
//            case 1:
//                y = 1;
//                break;
//        }

        //_guidedMissileController.Steer(x, y);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        _distance = CalculateDistance();

        if (_distance < 2f)
        {
            AddReward(_distance - _previousDistance);
        
            AddReward(_angle - _previousAngle);

            if (trainingMode)
            {
                Restart();
            }
        }
        
        Vector3 targetPosition = target.position;
        
        //position if _target with respect to vehicle (localized)
        Vector3 localTarget = transform.InverseTransformPoint(targetPosition);
        
        //1 observation
        sensor.AddObservation(_distance);
        
        //3 Observations
        sensor.AddObservation(localTarget);
        
        Vector3 direction = targetPosition - transform.position;
        
        //angle between forward of target and forward of missile
        Vector3 forward = transform.forward;

        _angle = CalculateAngle();
        
        //To determine if angle is positive or negative
        Vector3 cross = Vector3.Cross(forward, direction);
        
        //1 Observation
        sensor.AddObservation(Mathf.Sign(cross.y) * _angle);
        
        
        Vector3 localVelocity = transform.InverseTransformDirection(_guidedMissileController.GetVelocity());
        
        //3 observation
        sensor.AddObservation(localVelocity);
        
        //4 observations
        sensor.AddObservation(transform.localRotation.normalized);
        
        //add reward if target is further from missile and punish if closer
        AddReward(_distance - _previousDistance);
        
        _previousDistance = _distance;
        
        //add reward if angle is further to target and punish if closer
        AddReward(_angle - _previousAngle);
        
        _previousAngle = _angle;
    }

    private void Restart()
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(-100f, 100f);
        position.y = Random.Range(- 100f, 100f);
        position.z = Random.Range(-100f, 100f);

        transform.position = position;
        
        Vector3 targetPosition = Vector3.zero;

        targetPosition.x = Random.Range(-100f, 100f);
        targetPosition.y = Random.Range(- 100f, 100f);
        targetPosition.z = Random.Range(-100f, 100f);
        
        target.position = targetPosition;
     
        _guidedMissileController.Sleep();
        
        _distance = CalculateDistance();
        
        _previousDistance = _distance;

        _angle = CalculateAngle();

        _previousAngle = _angle;
    }
    
    private float CalculateDistance()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private float CalculateAngle()
    {
        return Vector3.Angle(transform.forward, target.position - transform.position);
    }
}