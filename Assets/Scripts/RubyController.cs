using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public int health { get { return currentHealth; } }
    int currentHealth;
    bool isInvincible;
    float invincibleTimer;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    public GameObject projectilePrefab;
    public GameObject healthIncrease;
    public GameObject healthDecrease;
    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    private int scoreValue;
    public Text score;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject trueTextObject;
    Scene m_Scene;
    string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        score.text = scoreValue.ToString();
        scoreValue = 0;
        changeScore(0);
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        trueTextObject.SetActive(false);
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {

            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 2.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {

                    character.DisplayDialog();

                }
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (currentHealth == 0)
        {
            loseTextObject.SetActive(true);
            GameObject background = GameObject.FindWithTag("BGmusic");
            if (background != null)
            {
                Debug.Log("found!!!");
                Destroy(background);
            }
            PlaySound(musicClipOne);
            audioSource.loop = false;
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        if (currentHealth == 0 && sceneName == "Help")
        {
            loseTextObject.SetActive(true);
            GameObject background = GameObject.FindWithTag("BGmusic");
            if (background != null)
            {
                Debug.Log("found!!!");
                Destroy(background);
            }
            PlaySound(musicClipOne);
            audioSource.loop = false;
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (scoreValue == 4)
        {
            winTextObject.SetActive(true);
            GameObject background = GameObject.FindWithTag("BGmusic");
            if (background != null)
            {
                Debug.Log("found!!!");
                Destroy(background);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                SceneManager.LoadScene("Help");
            }
            PlaySound(musicClipTwo);
            audioSource.loop = false;
        }

        if (scoreValue == 4 && sceneName == "Help")
        {
            trueTextObject.SetActive(true);
            winTextObject.SetActive(false);
            GameObject background = GameObject.FindWithTag("BGmusic");
            if (background != null)
            {
                Debug.Log("found!!!");
                Destroy(background);
            }
            PlaySound(musicClipTwo);
            audioSource.loop = false;
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject healthDown = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            GameObject healthUp = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        PlaySound(throwSound);
    }

    public void changeScore(int number)
    {
        scoreValue = scoreValue + number;
        score.text = "Robots Fixed: " + scoreValue.ToString();
    }


}
