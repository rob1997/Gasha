using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Trainer : MonoBehaviour
{
    public ProjectileMissileController projectileMissileController;
    
    //public TrackerAi trackerAi;
    
    //public Dome dome;

    private const string _groundTag = "TargetGround";
    
    private const string _guidedMissileTag = "GuidedMissile";

    private int _groundHit = 0;
    private int _interceptionHit = 0;
    
    private void Start()
    {
//        trackerAi.target = trackerAi.transform;
        
        ResetProjectileMissile();

//        dome.OnEntry += other => { trackerAi.target = other.transform; };
        
        projectileMissileController.OnDetonation += collision =>
        {
            if (collision.gameObject.CompareTag(_groundTag))
            {
                Debug.LogError($"Ground Hit : {++_groundHit}");
            }
                
            else if (collision.gameObject.CompareTag(_guidedMissileTag))
            {
                Debug.LogError($"Interception Hit : {++_interceptionHit}");
//                trackerAi.AddReward(.5f);
//                trackerAi.target = trackerAi.transform;
            }

            else
            {
                Debug.LogError($"Unknown Hit : {collision.gameObject.tag}");
            }
                
            ResetProjectileMissile();
        };
    }

    private void ResetProjectileMissile()
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(-100f, 200f);
        position.y = 2.75f;
        position.z = Random.Range(-100f, 200f);
        
        float x = position.x;
        float z = position.z;
        
        if (x > 0 && x < 100 && z > 0 && z < 100)
        {
            float random = Random.Range(0, 1);

            if (random >= .5f)
            {
                random = Random.Range(0, 1);

                if (random >= .5f)
                {
                    position.x += 100f;   
                }

                else
                {
                    position.x -= 100f;
                }
            }

            else
            {
                random = Random.Range(0, 1);

                if (random >= .5f)
                {
                    position.z += 100f;   
                }

                else
                {
                    position.z -= 100f;
                }
            }
        }
        
        projectileMissileController.transform.position = position;
        
        projectileMissileController.transform.rotation = Quaternion.LookRotation(Vector3.up);

        projectileMissileController.Sleep();
        
        projectileMissileController.Launch(Random.Range(15f, 75f), new Vector3(Random.Range(15f, 85f), 0, Random.Range(15f, 85f)));
    }
}
