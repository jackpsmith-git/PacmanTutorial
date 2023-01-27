using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacmanController : MonoBehaviour
{
    public GameObject canvas;
    public Animator textAnimator;
    public bool pressedYet;
    public float speed;
    public AudioSource introAudio;
    public AudioSource eatGhost;

    public GameObject red;
    public GameObject pink;
    public GameObject blue;
    public GameObject orange;
    public Collider2D pacmanCollider;

    // Start is called before the first frame update
    void Awake()
    {
        pacmanCollider = GetComponent<Collider2D>();
        introAudio.Play();
        speed = 3;
        pressedYet = false;
        textAnimator = canvas.GetComponentInChildren<Animator>();
        textAnimator.SetBool("freeze", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            textAnimator.SetBool("freeze", true);
            pressedYet = true;
        }

        if (pressedYet == true)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, 2f, speed * Time.deltaTime);
            transform.position = pos;
        }

        if (transform.position.x == 2)
        {
            introAudio.Stop();
            SceneManager.LoadScene("GameScene");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        eatGhost.Play();
        Destroy(col.gameObject);
    }

    
}

