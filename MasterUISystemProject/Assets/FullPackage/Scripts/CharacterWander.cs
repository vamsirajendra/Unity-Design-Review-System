﻿using UnityEngine;
using System.Collections;

public class CharacterWander : MonoBehaviour {

    public enum WanderMode { Idle, Patrol, Explore, Bookmark }

    public float localWanderRadius = 3;

    public Vector3 localWanderCenter = Vector3.zero;

    public Vector3 dropPoint = Vector3.zero;

    public WanderMode mode = WanderMode.Idle;
    private WanderMode prevMode = WanderMode.Idle;

    public float defaultSpeed = 1.5f;

    public float normalSpeedRadius = 3f;

    public float navSpeedRatio = 1;

    public int poiDestination = -1;

    private NavMeshAgent navAgent;

    private Animator animator;

    private NavMeshHit hit;

    private int idleTime = 0;

    public void SetWanderMode()
    {
        switch (mode)
        {
            case WanderMode.Idle:
                SetIdleMode();
                break;
            case WanderMode.Patrol:
                SetPatrolMode();
                break;
            case WanderMode.Explore:
                SetExploreMode();
                break;
            default:
                break;
        }
    }

    private void SetIdleMode()
    {
        if (navAgent.isOnNavMesh)
            navAgent.Stop();
        animator.SetFloat("Speed", 0f);
        animator.SetFloat("Direction", 0);
    }

    private void SetPatrolMode()
    {
        if (navAgent.isOnNavMesh)
            navAgent.Stop();
        CalcDestination();
        StartMovement();

    }

    private void SetExploreMode()
    {
        if (navAgent.isOnNavMesh)
            navAgent.Stop();
        CalcDestination();
        StartMovement();
    }

    public void SetBookmarkMode()
    {
        if (navAgent.isOnNavMesh)
            navAgent.Stop();

        prevMode = mode;
        mode = WanderMode.Bookmark;
        CalcDestination();
        StartMovement();

    }

    protected Vector3 CalcDestination()
    {
        Vector3 newDestination = new Vector3();

        switch(mode)
        {
            case WanderMode.Patrol:
                NavMesh.SamplePosition(localWanderCenter + new Vector3(Random.Range(-localWanderRadius, localWanderRadius), 0, Random.Range(-localWanderRadius, localWanderRadius)), out hit, 10, -1);
                break;
            case WanderMode.Explore:
                NavMesh.SamplePosition(transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), out hit, 10, -1);
                break;
            case WanderMode.Bookmark:
                NavMesh.SamplePosition(POIButtonManager.originalHandler.projectPOIs[poiDestination].position, out hit, 10, -1);
                break;
            default:
                break;
        }
        return newDestination;
    }

    protected void StartMovement()
    {
        navAgent.SetDestination(hit.position);
        navAgent.Resume();
        animator.SetFloat("Speed", 1f);
    }

    void OnAnimatorMove()
    {
        //only perform if moving
        if (!(navAgent.remainingDistance < .5f) && !navAgent.pathPending)
        {
            if (Vector3.Angle(transform.forward, navAgent.desiredVelocity) > 5)
                animator.SetFloat("Direction", Vector3.Angle(transform.forward, navAgent.destination - transform.position));
            else
                animator.SetFloat("Direction", 0);
            /*
            if (animator.GetFloat("Speed") == 1)
            {
                if (Vector3.Angle(transform.forward, navAgent.desiredVelocity) > 45)
                    animator.SetFloat("Direction", 90f);
                else if (Vector3.Angle(transform.forward, navAgent.desiredVelocity) < -45)
                    animator.SetFloat("Direction", -90f);
                else
                    animator.SetFloat("Direction", 0f);    
            }
            */
            animator.speed = navSpeedRatio;
            navAgent.velocity = animator.deltaPosition / Time.deltaTime * navSpeedRatio;
            navAgent.speed = navAgent.velocity.magnitude;

            //smoothly rotate the character in the desired direction of motion
            if (navAgent.desiredVelocity != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
            }
        }
    }

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (navAgent.remainingDistance < .5f && !navAgent.pathPending && !IsInvoking())
        {
            if (navAgent.isOnNavMesh)
                navAgent.Stop();
            animator.SetFloat("Speed", 0f);

            if (mode == WanderMode.Bookmark)
            {
                poiDestination = -1;
                mode = prevMode;
                localWanderCenter = transform.position;
            }

            Invoke("SetWanderMode", idleTime);
        }

    }

}