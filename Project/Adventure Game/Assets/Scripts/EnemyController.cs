using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public float changeTime = 2.0f;
    float timer;

    private int directionIndex = 0;
    private Vector2[] directions = 
    {
        Vector2.right,
        Vector2.up,
        Vector2.left,
        Vector2.down
    };

    Animator animator;

    Rigidbody2D rigidbody2d;

    bool broken = true;
    public AudioSource walkAndHitAudioSource;
    public AudioSource fixAudioSource;
    public AudioClip enemyHit;

    public ParticleSystem smokeEffect;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;

        animator = GetComponent<Animator>();
        walkAndHitAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            directionIndex += 1;
            if (directionIndex >= directions.Length)
            {
                directionIndex = 0;
            }
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 direction = directions[directionIndex];
        rigidbody2d.velocity = direction * speed;

        animator.SetFloat("Move X", direction.x);
        animator.SetFloat("Move Y", direction.y);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        if (controller != null) 
        {
            controller.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        if (!broken)
        {
            return;
        }
        broken = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();

        walkAndHitAudioSource.Stop();
        walkAndHitAudioSource.PlayOneShot(enemyHit);

        fixAudioSource.Play();

        rigidbody2d.velocity = Vector2.zero;
        rigidbody2d.simulated = false;


    }
}
