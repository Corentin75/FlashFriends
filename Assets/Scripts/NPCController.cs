using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [Header("Routine")]
    public POI[] routine;

    [Header("Player Detection")]
    public Camera playerCamera;
    public float detectionDistance = 20f;

    [Header("Reaction Settings")]
    public float reactionDuration = 5f; // durée fixe pour toutes les réactions
    public float fleeDistance = 5f;
    public float normalSpeed = 3.5f;
    public float fleeSpeed = 7f;

    private NavMeshAgent agent;
    private Animator animator;

    private int currentPOI = 0;
    private POI currentTarget;

    private bool isReacting = false;
    private Coroutine routineCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.avoidancePriority = Random.Range(20, 80);
        agent.speed = normalSpeed;
    }

    void Start()
    {
        if (routine.Length > 0)
            routineCoroutine = StartCoroutine(FollowRoutine());
    }

    void Update()
    {
        UpdateAnimation();
    }

    // ---------------- ROUTINE ----------------
    IEnumerator FollowRoutine()
    {
        while (true)
        {
            if (routine.Length == 0)
                yield break;

            currentTarget = routine[currentPOI];

            Vector3 destination = currentTarget.GetRandomPoint();
            agent.isStopped = false;
            agent.SetDestination(destination);

            // attendre que le chemin soit calculé
            while (agent.pathPending)
                yield return null;

            // attendre que le PNJ arrive réellement
            while (agent.remainingDistance > agent.stoppingDistance || agent.velocity.sqrMagnitude > 0.01f)
            {
                if (isReacting) yield return null;
                yield return null;
            }

            // PNJ arrivé → appliquer comportement POI
            HandlePOIBehaviour(true);

            float waitTime = currentTarget.waitTime;
            float timer = 0f;

            while (timer < waitTime)
            {
                if (isReacting) yield return null;

                timer += Time.deltaTime;
                yield return null;
            }

            HandlePOIBehaviour(false);

            currentPOI++;
            if (currentPOI >= routine.Length)
                currentPOI = 0;
        }
    }

    // ---------------- PHOTO REACTION ----------------
    public void OnPlayerPhotographed()
    {
        if (!isReacting)
            StartCoroutine(ReactToPhoto());
    }

    IEnumerator ReactToPhoto()
    {
        isReacting = true;

        if (routineCoroutine != null)
            StopCoroutine(routineCoroutine);

        // Stop complet du NavMeshAgent pendant la réaction
        agent.isStopped = true;
        agent.ResetPath();

        ResetReactionTriggers();

        int reaction = Random.Range(0, 4); // 0=Floss,1=Hype,2=SelfCheck,3=Ignore

        switch (reaction)
        {
            case 0: // Floss
                animator.SetTrigger("FlossTrigger");
                SoundManager.Instance.Play(SoundManager.Instance.flossDanceSfx);
                break;

            case 1: // Hype dance
                animator.SetTrigger("HypeTrigger");
                SoundManager.Instance.Play(SoundManager.Instance.hypeDanceSfx);
                break;

            case 2: // SelfCheck
                animator.SetTrigger("SelfCheckTrigger");
                SoundManager.Instance.Play(SoundManager.Instance.selfCheckSfx);
                break;

            case 3: // Ignore
                // rien, on continue la routine après
                SoundManager.Instance.Play(SoundManager.Instance.ignoreSfx);
                break;
        }

        // attendre la durée fixe
        yield return new WaitForSeconds(reactionDuration);

        ResetReactionTriggers();

        // reprendre la routine
        agent.speed = normalSpeed;
        agent.isStopped = false;
        isReacting = false;

        routineCoroutine = StartCoroutine(FollowRoutine());
    }

    void ResetReactionTriggers()
    {
        animator.ResetTrigger("FlossTrigger");
        animator.ResetTrigger("HypeTrigger");
        animator.ResetTrigger("SelfCheckTrigger");
    }

    public int GetPoseScore()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("StageDance"))
            return 5;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SelfCheck"))
            return 10;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("FlossDance"))
            return 20;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HypeDance"))
            return 30;
        // Idle ou autre
        return 0;
    }

    // ---------------- POI BEHAVIOUR ----------------
    void HandlePOIBehaviour(bool entering)
    {
        if (animator == null || currentTarget == null) return;

        if (currentTarget.type == POI.POIType.Stage)
        {
            if (entering) transform.LookAt(currentTarget.transform); // regarder la scène
            animator.SetBool("isDancing", entering);
            agent.isStopped = entering;
        }
    }

    // ---------------- ANIMATION ----------------
    void UpdateAnimation()
    {
        if (animator == null) return;

        if (isReacting)
        {
            // Stop complet pendant la réaction (Floss / SelfCheck)
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            // vitesse normale (Idle/Walk)
            float speed = (!agent.isStopped && agent.hasPath) ? agent.desiredVelocity.magnitude : 0f;
            animator.SetFloat("Speed", speed);
        }
    }
}
