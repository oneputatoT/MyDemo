using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    Animator animator;
    float startAnimTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        startAnimTime = Time.time;
    }

    
    void Update()
    {
        float duration = Time.time- startAnimTime;

        if (duration >= animator.GetCurrentAnimatorStateInfo(0).length)
        {
            GameManager.instance.SubAnimation();
            Destroy(gameObject);
        }
    }
}
