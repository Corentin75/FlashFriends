using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference takePhotoAction;
    [SerializeField] private InputActionReference openAlbumAction;
    [SerializeField] private InputActionReference openQuestsAction;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float gravity = -9.81f;
    public float groundStickForce = -2f;
    private Vector3 velocity;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraPivot;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    private float xRotation;

    [Header("Photo Settings")]
    public Camera playerCamera;
    public Camera photoCamera;
    public RenderTexture photoRenderTexture;
    public float photoRange = 20f;
    public LayerMask photoLayer;
    public UnityEngine.UI.Image flashImage;
    public float flashDuration = 0.1f;
    public float photoAngle = 40f; // half-angle of detection cone

    private CharacterController controller;
    private Animator animator;

    public bool gameplayActive = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();

        takePhotoAction.action.Enable();
        takePhotoAction.action.performed += OnPhotoTaken;

        openAlbumAction.action.Enable();
        openAlbumAction.action.performed += OnToggleAlbum;

        openQuestsAction.action.Enable();
        openQuestsAction.action.performed += OnToggleQuest;
    }

    private void OnDisable()
    {
        takePhotoAction.action.performed -= OnPhotoTaken;
        openAlbumAction.action.performed -= OnToggleAlbum;
        openQuestsAction.action.performed -= OnToggleQuest;

        moveAction.action.Disable();
        lookAction.action.Disable();
        takePhotoAction.action.Disable();
        openAlbumAction.action.Disable();
        openQuestsAction.action.Disable();
    }

    // ---------------- GAMEPLAY STATE ----------------
    public void SetGameplayActive(bool active)
    {
        gameplayActive = active;

        if (active)
        {
            moveAction.action.Enable();
            lookAction.action.Enable();
            takePhotoAction.action.Enable();
        }
        else
        {
            moveAction.action.Disable();
            lookAction.action.Disable();
            takePhotoAction.action.Disable();
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateAnimation();
    }

    // ---------------- MOVEMENT ----------------
    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.forward * input.y + transform.right * input.x;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = groundStickForce;

        velocity.y += gravity * Time.deltaTime;
        controller.Move((move * moveSpeed + velocity) * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool isWalking = input.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isWalking);
    }

    // ---------------- LOOK ----------------
    private void HandleLook()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>();
        float mouseX = look.x * mouseSensitivity * Time.deltaTime;
        float mouseY = look.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ---------------- PHOTO ----------------
    private void OnPhotoTaken(InputAction.CallbackContext context)
    {
        if (gameplayActive)
            TakePhoto();
    }

    private void TakePhoto()
    {
        int photoScore = 0;
        HashSet<GameObject> objectsInPhoto = new HashSet<GameObject>();

        Collider[] potentialObjects = Physics.OverlapSphere(playerCamera.transform.position, photoRange, photoLayer);
        float maxAngle = photoAngle;

        foreach (var col in potentialObjects)
        {
            Vector3 toObject = col.transform.position - playerCamera.transform.position;

            if (Vector3.Angle(playerCamera.transform.forward, toObject) <= maxAngle)
            {
                if (!objectsInPhoto.Contains(col.gameObject))
                {
                    objectsInPhoto.Add(col.gameObject);

                    NPCController npc = col.GetComponent<NPCController>();
                    if (npc != null)
                        npc.OnPlayerPhotographed();
                }
            }
        }

        // Captures photo
        PhotoManager.Instance?.CapturePhoto(photoScore);

        // Calculates score
        if (QuestManager.Instance != null)
        {
            photoScore = QuestManager.Instance.CheckPhotoAndReturnScore(new List<GameObject>(objectsInPhoto));

            if (GameManager.Instance.currentState == GameState.Playing)
                PhotoFeedbackManager.Instance.ShowFeedback(photoScore);
        }

        StartCoroutine(PhotoFlashEffect());
    }

    // ---------------- PHOTO FLASH ----------------
    private IEnumerator PhotoFlashEffect()
    {
        if (flashImage == null) yield break;

        SoundManager.Instance.Play(SoundManager.Instance.photoSfx);

        flashImage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSecondsRealtime(flashDuration);

        float fadeSpeed = 5f;
        while (flashImage.color.a > 0f)
        {
            Color c = flashImage.color;
            c.a -= fadeSpeed * Time.unscaledDeltaTime;
            flashImage.color = c;
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }

    // ---------------- ALBUM ----------------
    private void OnToggleAlbum(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (GameManager.Instance.currentState == GameState.Playing)
            GameManager.Instance.OpenAlbum();
        else if (GameManager.Instance.currentState == GameState.Album)
            GameManager.Instance.CloseAlbum();
    }

    // ---------------- QUEST PANEL ----------------
    private void OnToggleQuest(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (GameManager.Instance.currentState == GameState.Playing)
            GameManager.Instance.OpenQuestPanel();
        else if (GameManager.Instance.currentState == GameState.Quest)
            GameManager.Instance.CloseQuestPanel();
    }

    // ---------------- DEBUG ----------------
    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;

        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Vector3 origin = playerCamera.transform.position;
        Vector3 forward = playerCamera.transform.forward;

        int segments = 20;
        for (int i = 0; i <= segments; i++)
        {
            float theta = -photoAngle + (i / (float)segments) * 2f * photoAngle;
            Vector3 dir = Quaternion.AngleAxis(theta, playerCamera.transform.up) * forward;
            Gizmos.DrawLine(origin, origin + dir.normalized * photoRange);

            dir = Quaternion.AngleAxis(theta, playerCamera.transform.right) * forward;
            Gizmos.DrawLine(origin, origin + dir.normalized * photoRange);
        }
    }
}
