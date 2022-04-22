using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator animator;
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;
    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;
    public ParticleSystem smokeEffect;
    public ParticleSystem HealthDecrease;
    private RubyController rubyController;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");

        if (rubyControllerObject != null)

        {

            rubyController = rubyControllerObject.GetComponent<RubyController>();

            print("Found the RubyConroller Script!");

        }

        if (rubyController == null)

        {

            print("Cannot find GameController Script!");

        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (!broken)
        {
            return;
        }

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {

        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2D.position;
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        smokeEffect.Stop();
        animator.SetTrigger("Fixed");
        rubyController.changeScore(1);
    }


}
