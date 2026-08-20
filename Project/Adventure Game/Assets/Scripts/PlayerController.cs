using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction talkAction;
    public Rigidbody2D rigidbody2d;
    public Vector2 move;
    public float speed = 3f; 
    public float timeInvicible = 2f;
    public bool isInvicible;
    public float damageCooldown;

    public int maxHealth = 5;
    public int health 
    {
        get
        {
            return currentHealth;
        }
    }
    private int currentHealth;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;

    public AudioSource walkAudioSource;
    public AudioSource sfxAudioSource;

    public AudioClip playerWalk;
    public AudioClip throwProjectile;

    // Start is called before the first frame update
    void Start()
    {
        MoveAction.Enable();
        talkAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        walkAudioSource.clip = playerWalk;
    }

    // Update is called once per frame
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        //Debug.Log(move);
        if (isInvicible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0) 
            { 
                isInvicible = false;
            }
        }

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
            moveDirection.Set(move.x,move.y);
            moveDirection.Normalize();
        }
        else
        {
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }


        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            FindFriend();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvicible)
            {
                return;
            }
            isInvicible = true;
            damageCooldown = timeInvicible;
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);
        animator.SetTrigger("Launch");
        sfxAudioSource.PlayOneShot(throwProjectile);
    }

    void FindFriend()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
            if (character != null) 
            {
                UIHandler.instance.DisplayDialogue();
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
}
