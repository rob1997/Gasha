using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Trainer : MonoBehaviour
{
    public List<ProjectileMissileController> projectileMissileControllers;
    public Transform crust;
    
    //public TrackerAi trackerAi;
    
    //public Dome dome;

    private const string _groundTag = "TargetGround";
    
    private const string _guidedMissileTag = "GuidedMissile";

    private int _groundHit = 0;
    private int _interceptionHit = 0;
    
    private void Start()
    {
        projectileMissileControllers.ForEach(projectileMissileController => 
        { 
            ResetProjectileMissile(projectileMissileController);
            
            projectileMissileController.OnDetonation += collision =>
            {
                if (collision.gameObject.CompareTag(_groundTag))
                {
                    Debug.LogError($"Ground Hit : {++_groundHit}");
                }
                
                else if (collision.gameObject.CompareTag(_guidedMissileTag))
                {
                    Debug.LogError($"Interception Hit : {++_interceptionHit}");
                }

                else
                {
                    Debug.LogError($"Unknown Hit : {collision.gameObject.tag}");
                }
                
                ResetProjectileMissile(projectileMissileController);
            };
        });
    }

    private void ResetProjectileMissile(ProjectileMissileController projectileMissileController)
    {
        Transform childCrust = crust.GetChild(Random.Range(0, crust.childCount));

        Vector3 position = childCrust.TransformPoint(new Vector3(Random.Range(0, 100f), 5f, Random.Range(0, 100f)));

        projectileMissileController.transform.position = position;
        
        projectileMissileController.transform.rotation = Quaternion.LookRotation(Vector3.up);

        projectileMissileController.Sleep();
        
        projectileMissileController.Launch(Random.Range(15f, 75f), new Vector3(Random.Range(15f, 85f), 0, Random.Range(15f, 85f)));
    }
}
