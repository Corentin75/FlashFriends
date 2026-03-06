using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference takePhotoAction;
    [SerializeField] private InputActionReference openAlbumAction;

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

    [Header("Photo Settings")]
    public Camera playerCamera;
    public Camera photoCamera;
    public RenderTexture photoRenderTexture;
    public float photoRange = 20f;
    public LayerMask photoLayer;
    public Image flashImage;
    public float flashDuration = 0.1f;

    private CharacterController controller;
    private float xRotation;
    private Animator animator;

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
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        takePhotoAction.action.performed -= OnPhotoTaken;
        takePhotoAction.action.Disable();
        openAlbumAction.action.performed -= OnToggleAlbum;
        openAlbumAction.action.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateAnimation();
    }

    public void SetGameplayActive(bool active)
    {
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

    // ---------------- MOVEMENT ----------------

    private void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.forward * input.y + transform.right * input.x;

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = groundStickForce;
        }

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
        TakePhoto();
    }

    private void TakePhoto()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 0.5f, photoRange, photoLayer);

        HashSet<GameObject> objectsInPhoto = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            GameObject obj = hit.collider.gameObject;

            if (!objectsInPhoto.Contains(obj))
            {
                objectsInPhoto.Add(obj);

                NPCController npc = obj.GetComponent<NPCController>();
                if (npc != null)
                    npc.OnPlayerPhotographed();
            }
        }

        int photoScore = 0;

        // Vérification quêtes + scoring
        if (QuestManager.Instance != null)
        {
            // Retourne le score de cette photo
            photoScore = QuestManager.Instance.CheckPhotoAndReturnScore(new List<GameObject>(objectsInPhoto));
        }

        // Capture la photo et passe le score pour la sauvegarde
        if (PhotoManager.Instance != null)
        {
            PhotoManager.Instance.CapturePhoto(photoScore);
        }

        StartCoroutine(PhotoFlashEffect());
    }

    private IEnumerator PhotoFlashEffect()
    {
        if (flashImage == null)
            yield break;

        SoundManager.Instance.Play(SoundManager.Instance.photoSfx);

        // flash blanc instantané
        flashImage.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(flashDuration);

        // fade out
        float fadeSpeed = 5f;

        while (flashImage.color.a > 0)
        {
            Color c = flashImage.color;
            c.a -= fadeSpeed * Time.deltaTime;
            flashImage.color = c;
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }

    // ---------------- ALBUM ----------------

    private void OnToggleAlbum(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AlbumUIController.Instance.ToggleAlbum();
        }
    }
}
