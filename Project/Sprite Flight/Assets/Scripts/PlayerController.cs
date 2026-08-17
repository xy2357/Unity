using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime;
    public float thrustForce = 2f;
    public float maxSpeed = 10f;
    public float score;
    public float scoreMultiplier = 10f;
    public float highScore;
    public GameObject boosterFlame;
    public GameObject explosionEffect;
    public GameObject borderParent;
    public UIDocument uiDocument;
    private Label scoreText;
    private Label highText;
    public Button restartButton;
    public InputAction moveForward;
    public InputAction lookPosition;
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");

        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        highText.text = "High Score" + highScore;

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        moveForward.Enable();
        lookPosition.Enable();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateScore();

        MovePlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();

            highText.text = "High Score:" + highScore;
        }

        Destroy(gameObject);
        Instantiate(explosionEffect, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;

        borderParent.SetActive(false);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateScore()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score:" + score;
    }

    void MovePlayer()
    {
        if (moveForward.IsPressed())
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(lookPosition.ReadValue<Vector2>());
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (moveForward.WasPressedThisFrame())
        {
            boosterFlame.SetActive(true);
        }
        else if (moveForward.WasReleasedThisFrame())
        {
            boosterFlame.SetActive(false);
        }
    }
}
