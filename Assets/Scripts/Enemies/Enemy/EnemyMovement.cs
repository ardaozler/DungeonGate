using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class EnemyMovement : MonoBehaviour
{
    [FormerlySerializedAs("Player")] public Transform player;
    [FormerlySerializedAs("UpdateRate")] public float updateRate = 0.1f;
    private NavMeshAgent _agent;
    private AgentLinkMover _linkMover;

    private Coroutine _followCoroutine;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _linkMover = GetComponent<AgentLinkMover>();

        _linkMover.OnLinkStart += HandleLinkStart;
        _linkMover.OnLinkEnd += HandleLinkEnd;
    }

    public void StartChasing()
    {
        if (_followCoroutine == null)
        {
            _followCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateRate);

        while (gameObject.activeSelf)
        {
            _agent.SetDestination(player.transform.position);
            yield return Wait;
        }
    }

    private void HandleLinkStart()
    {
     //   Animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
       // Animator.SetTrigger(Landed);
    }
}
