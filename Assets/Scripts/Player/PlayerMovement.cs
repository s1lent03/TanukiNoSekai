using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TerrainSurface
{

    public static float[] GetTextureMix(Terrain terrain, Vector3 worldPos)
    {

        // returns an array containing the relative mix of textures
        // on the main terrain at this world position.

        // The number of values in the array will equal the number
        // of textures added to the terrain.

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        // calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int n = 0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }

        return cellMix;
    }

    public static int GetMainTexture(Terrain terrain, Vector3 worldPos)
    {

        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.

        float[] mix = GetTextureMix(terrain, worldPos);

        float maxMix = 0;
        int maxIndex = 0;

        // loop through each mix value and find the maximum
        for (int n = 0; n < mix.Length; ++n)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }

        return maxIndex;
    }
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float crouchSpeed;
    public float normalSpeed;
    public float sprintSpeed;
    public float speed;
    public float rotationSpeed;
    public float gravity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Sons")]
    public AudioClip[] dirtWalk;
    public AudioClip[] grassWalk;
    public AudioClip[] woodWalk;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private float height = 0f;
    private bool isGrounded;
    private AudioClip[] currentGround;

    private bool canPlaySound = true;
    public float walkTimeBetweenSteps;
    public float runTimeBetweenSteps;

    [Header("Others")]
    //Parar movimento se o jogo estiver em pausa
    public bool isPaused;
    public bool chatCooldown;
    public GameObject pauseMenu;
    public Animator animator;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0.15f, 1.35f, 0), 0.25f);
    }

    private void obtainSound(AudioClip[] array)
    {
        AudioClip sound = array[Random.Range(0, array.Length)];
        GetComponent<AudioSource>().clip = sound;
    }

    private void Awake()
    {
        chatCooldown = false;
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position - new Vector3(0.15f, 1.35f, 0), 0.25f, 1 << 3);

        if (colliders.Length > 0)
        {
            if (!colliders[0].GetComponent<Terrain>())
            {
                currentGround = woodWalk;
            } else
            {
                var surfaceIndex = TerrainSurface.GetMainTexture(colliders[0].GetComponent<Terrain>(), transform.position);

                if (surfaceIndex == 0)
                {
                    currentGround = grassWalk;
                } else
                {
                    currentGround = dirtWalk;
                }
            }
        } else if (currentGround == null)
        {
            currentGround = dirtWalk;
        }
        
        //Não deixar o jogador se mexer caso o jogo esteja em pausa
        if (isPaused != true)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();

            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;

            forward = forward.normalized;
            right = right.normalized;

            Vector3 direction = (move.x * right) + (move.y * forward);

            if (!isGrounded)
            {
                height += gravity * Time.deltaTime;
                direction.y = height;
            }

            if (playerInput.actions["Crouch"].IsPressed())
                speed = crouchSpeed;
            else if (playerInput.actions["Sprint"].IsPressed() && move.y > 0)
                speed = sprintSpeed;
            else
                speed = normalSpeed;

            if (move != Vector2.zero && !GetComponent<AudioSource>().isPlaying)
            {
                StartCoroutine(PlaySound());
            }

            animator.SetBool(Animator.StringToHash("Moving"), move != Vector2.zero);
            animator.SetBool(Animator.StringToHash("Running"), speed == sprintSpeed);
            animator.SetBool(Animator.StringToHash("Crouching"), playerInput.actions["Crouch"].IsPressed());

            characterController.Move(direction * speed * Time.deltaTime);

            Quaternion toRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        } else
        {
            animator.SetBool(Animator.StringToHash("Moving"), false);
            animator.SetBool(Animator.StringToHash("Running"), false);
            animator.SetBool(Animator.StringToHash("Crouching"), false);
        }
    }

    IEnumerator PlaySound()
    {
        if (canPlaySound && playerInput.actions["Sprint"].IsPressed())
        {
            obtainSound(currentGround);
            GetComponent<AudioSource>().Play();

            canPlaySound = false;
            yield return new WaitForSeconds(runTimeBetweenSteps);
            canPlaySound = true;
        }
        else if (canPlaySound && !playerInput.actions["Sprint"].IsPressed())
        {
            obtainSound(currentGround);
            GetComponent<AudioSource>().Play();

            canPlaySound = false;
            yield return new WaitForSeconds(walkTimeBetweenSteps);
            canPlaySound = true;
        }    
    }
}
