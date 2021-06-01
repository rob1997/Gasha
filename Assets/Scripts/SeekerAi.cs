using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class SeekerAi : Agent
{
    private GuidedMissileController _guidedMissileController;

    public Transform target;
    public bool trainingMode = true;
    
    public float lowerBound = - 100f;
    public float upperBound = 100f;

    private ProjectileMissileController _projectileMissileController;
    
    private Rigidbody _targetRigidBody;
    
    private float _distance;
    private float _previousDistance;
    
    private float _angle;
    private float _previousAngle;

    private float _targetDestinationY;
    
    public override void Initialize()
    {
        _guidedMissileController = GetComponent<GuidedMissileController>();

        if (trainingMode)
        {
            _projectileMissileController = target.GetComponent<ProjectileMissileController>();
            _targetRigidBody = target.GetComponent<Rigidbody>();
        }
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
        float power = Mathf.Clamp(actions.ContinuousActions[0], 0, 1f);
        
        _guidedMissileController.Launch(power);
        
        float x = Mathf.Clamp(actions.ContinuousActions[1], - 1f, 1f);
        float y = Mathf.Clamp(actions.ContinuousActions[2], - 1f, 1f);
        
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

        _guidedMissileController.Steer(x, y);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 targetPosition = target.position;
        
        _distance = CalculateDistance();

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
        
        if (_distance < 2f || targetPosition.y < _targetDestinationY)
        {
            AddReward(_previousDistance - _distance);
            
            AddReward(_previousAngle - _angle);

            if (trainingMode)
            {
                Restart();
            }
        }

        else
        {
            //add reward if target is closer from missile and punish if further
            AddReward(_previousDistance - _distance);
        
            _previousDistance = _distance;
        
            //add reward if angle is closer to target and punish if further
            AddReward(_previousAngle - _angle);
        
            _previousAngle = _angle;
        }
    }

    private void Restart()
    {
        ResetPosition();
        
        LaunchTarget();
        
        _guidedMissileController.Sleep();
        
        Recalculate();
    }

    private void LaunchTarget()
    {
        _targetDestinationY = Random.Range(lowerBound, target.position.y);
        
        _projectileMissileController.Sleep();

        Vector3 targetDestination = GetRandomV3InBounds();

        targetDestination.y = _targetDestinationY;
        
        _projectileMissileController.Launch(Random.Range(15f, 75f), targetDestination);
    }
    
    private void ResetPosition()
    {
        transform.position = GetRandomV3InBounds();
        
        target.position = GetRandomV3InBounds();
    }
    
    private void Recalculate()
    {
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

    private float GetRandomInBounds()
    {
        return Random.Range(lowerBound, upperBound);
    }
    
    private Vector3 GetRandomV3InBounds()
    {
        return new Vector3(GetRandomInBounds(), GetRandomInBounds(), GetRandomInBounds());
    }
}