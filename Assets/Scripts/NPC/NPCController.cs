using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [Header("Routine")]
    public POI[] routine; // NPCs points of interest

    [Header("Player Detection")]
    public Camera playerCamera;
    public float detectionDistance = 20f;

    [Header("Reaction Settings")]
    public float reactionDuration = 5f; // time for each reaction
    public float fleeDistance = 5f;
    public float normalSpeed = 3.5f;
    public float fleeSpeed = 7f;

    private NavMeshAgent agent;
    private Animator animator;

    private int currentPOI = 0;
    private POI currentTarget;

    private bool isReacting = false;
    private Coroutine routineCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.avoidancePriority = Random.Range(20, 80);
        agent.speed = normalSpeed;
    }

    private void Start()
    {
        if (routine.Length > 0)
            routineCoroutine = StartCoroutine(FollowRoutine());
    }

    private void Update()
    {
        UpdateAnimation();
    }

    // ---------------- ROUTINE ----------------
    // NPC follows the assigned routine (POIs)
    private IEnumerator FollowRoutine()
    {
        while (true)
        {
            if (routine.Length == 0)
                yield break;

            currentTarget = routine[currentPOI];
            Vector3 destination = currentTarget.GetRandomPoint();

            agent.isStopped = false;
            agent.SetDestination(destination);

            // Waits until path is calculated
            while (agent.pathPending)
                yield return null;

            // Waits until NPC reaches destination
            while (agent.remainingDistance > agent.stoppingDistance || agent.velocity.sqrMagnitude > 0.01f)
            {
                if (isReacting) yield return null;
                yield return null;
            }

            HandlePOIBehaviour(true);

            float timer = 0f;
            while (timer < currentTarget.waitTime)
            {
                if (isReacting) yield return null;
                timer += Time.deltaTime;
                yield return null;
            }

            HandlePOIBehaviour(false);

            currentPOI = (currentPOI + 1) % routine.Length; // loop routine
        }
    }

    // ---------------- PHOTO REACTION ----------------
    public void OnPlayerPhotographed()
    {
        if (!isReacting)
            StartCoroutine(ReactToPhoto());
    }

    private IEnumerator ReactToPhoto()
    {
        isReacting = true;

        if (routineCoroutine != null)
            StopCoroutine(routineCoroutine);

        agent.isStopped = true;
        agent.ResetPath();

        ResetReactionTriggers();

        // Chooses a random reaction
        int reaction = Random.Range(0, 4); // 0=Floss, 1=Hype, 2=SelfCheck, 3=Ignore
        switch (reaction)
        {
            case 0:
                animator.SetTrigger("FlossTrigger");
                SoundManager.Instance.Play(SoundManager.Instance.flossDanceSfx);
                break;
            case 1:
                animator.SetTrigger("HypeTrigger");
                SoundManager.Instance.Play(SoundManager.Instance.hypeDanceSfx);
                break;
            case 2:
                animator.SetTrigger("SelfCheckTrigger");
                SoundManager.Instance.Play(SoundManager.Instance.selfCheckSfx);
                break;
            case 3:
                SoundManager.Instance.Play(SoundManager.Instance.ignoreSfx);
                break;
        }

        yield return new WaitForSeconds(reactionDuration);

        ResetReactionTriggers();

        // Resumes routine
        agent.speed = normalSpeed;
        agent.isStopped = false;
        isReacting = false;

        routineCoroutine = StartCoroutine(FollowRoutine());
    }

    private void ResetReactionTriggers()
    {
        animator.ResetTrigger("FlossTrigger");
        animator.ResetTrigger("HypeTrigger");
        animator.ResetTrigger("SelfCheckTrigger");
    }

    // ---------------- SCORING ----------------
    public int GetPoseScore()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("StageDance")) return 5;
        if (state.IsName("SelfCheck")) return 10;
        if (state.IsName("FlossDance")) return 20;
        if (state.IsName("HypeDance")) return 30;

        return 0; // idle or ignore
    }

    // ---------------- POI BEHAVIOUR ----------------
    private void HandlePOIBehaviour(bool entering)
    {
        if (currentTarget == null) return;

        if (currentTarget.type == POI.POIType.Stage)
        {
            if (entering) transform.LookAt(currentTarget.transform);
            animator.SetBool("isDancing", entering);
            agent.isStopped = entering;
        }
    }

    // ---------------- ANIMATION ----------------
    private void UpdateAnimation()
    {
        if (isReacting)
        {
            animator.SetFloat("Speed", 0f); // freeze during reaction
        }
        else
        {
            float speed = (!agent.isStopped && agent.hasPath) ? agent.desiredVelocity.magnitude : 0f;
            animator.SetFloat("Speed", speed);
        }
    }
}
