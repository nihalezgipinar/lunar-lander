using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public TextMeshProUGUI scorebar;
    public TextMeshProUGUI highscoreText;
    public MSIndicator msIndicator;

    Rigidbody2D rb2D;
    Vector2 hz;
    Vector2 ver;
    float fuel;
    public Image Fuelbar;

    int score;
    int highScore;

    public GameObject explosion;
    public GameObject mainfire;
    public GameObject rightfire;
    public GameObject leftfire;
    public float hardThreshold = 5f;
    public float crashThreshold = 8f;
    public float topContactMinY = 0.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fuel = 100;
        hz = new Vector2(50f, 0f);
        ver = new Vector2(0f, 50f);
        rb2D = GetComponent<Rigidbody2D>();

        //PlayerPrefs.DeleteAll();

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    // Update is called once per frame
    void Update()
    {

        float vSpeed = Mathf.Abs(rb2D.linearVelocity.y);
        Fuelbar.fillAmount = fuel / 100;

        scorebar.text = "Score " + score.ToString();
        UpdateHighScore();

        if (fuel >= 0)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb2D.AddForce(ver * Time.deltaTime);
                //Debug.Log("W");
                fuel -= (10) * Time.deltaTime;

                msIndicator.IsReceivingPlayerInput = true;

                mainfire.SetActive(true);

                return;
            }

            if (Input.GetKey(KeyCode.D))
            {
                rb2D.AddForce(hz * Time.deltaTime);
                fuel -= (10) * Time.deltaTime;
                msIndicator.IsReceivingPlayerInput = true;
                leftfire.SetActive(true);
                return;

            }
            if (Input.GetKey(KeyCode.A))
            {
                rb2D.AddForce(-hz * Time.deltaTime);
                fuel -= (10) * Time.deltaTime;
                msIndicator.IsReceivingPlayerInput = true;
                rightfire.SetActive(true);
                return;
            }
            mainfire.SetActive(false);
            leftfire.SetActive(false);
            rightfire.SetActive(false);

            msIndicator.IsReceivingPlayerInput = false;

        }


    }
    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;
        bool fromAbove = true;
        Debug.Log($"Your speed is {impactSpeed}");
        if (collision.contactCount > 0)
        {
            Vector2 n = collision.GetContact(0).normal;
            fromAbove = n.y > topContactMinY;
        }
        /**/

        bool isPad =
            collision.collider.CompareTag("landing") ||
            collision.collider.CompareTag("landing2") ||
            collision.collider.CompareTag("landing3");

        // Crash conditions: not a pad, or not from above, or too fast
        float softMult = 1f - Mathf.Clamp01(impactSpeed / hardThreshold);
        if (!isPad || !fromAbove || impactSpeed > crashThreshold)
        {

            fuel -= 10;
            //Debug.Log("ground");
            StartCoroutine(DestroyDestructable());

        }
        if (isPad && impactSpeed > crashThreshold)
        {

            fuel -= 10;
            //Debug.Log("ground");
            StartCoroutine(DestroyDestructable());

        }

        if (collision.gameObject.CompareTag("landing") && fromAbove && crashThreshold > impactSpeed)
        {
            var hasHitGreen = msIndicator.CheckIfInGreen();

            fuel = Mathf.Clamp(fuel + 10f, 0f, 100f);

            score += 100;
            //Debug.Log("landing");
            yield return new WaitForSeconds(.5f);
            transform.position = new Vector2(-3, 2);
            int pts = Mathf.RoundToInt(score * softMult);
            int fuelG = Mathf.RoundToInt(fuel * softMult);
            score += pts;
            fuel = Mathf.Clamp(fuel + fuelG, 0f, 100f);

            if (hasHitGreen)
            {
                pts += 50;
            }

            msIndicator.ResetIndicator();

        }
        if (collision.gameObject.CompareTag("landing2") && fromAbove && crashThreshold > impactSpeed)
        {
            var hasHitGreen = msIndicator.CheckIfInGreen();

            fuel = Mathf.Clamp(fuel + 30f, 0f, 100f); score += 300;

            //Debug.Log("landing2");
            yield return new WaitForSeconds(.5f);
            transform.position = new Vector2(-3, 2);
            int pts = Mathf.RoundToInt(score * softMult);
            int fuelG = Mathf.RoundToInt(fuel * softMult);
            score += pts;
            fuel = Mathf.Clamp(fuel + fuelG, 0f, 100f);

            if (hasHitGreen)
            {
                pts += 50;
            }

            msIndicator.ResetIndicator();
        }
        if (collision.gameObject.CompareTag("landing3") && fromAbove && crashThreshold > impactSpeed)
        {
            var hasHitGreen = msIndicator.CheckIfInGreen();

            fuel = Mathf.Clamp(fuel + 20f, 0f, 100f); score += 200;
            //Debug.Log("landing3");
            yield return new WaitForSeconds(.5f);
            transform.position = new Vector2(-3, 2);
            int pts = Mathf.RoundToInt(score * softMult);
            int fuelG = Mathf.RoundToInt(fuel * softMult);

            score += pts;
            fuel = Mathf.Clamp(fuel + fuelG, 0f, 100f);

            if (hasHitGreen)
            {
                pts += 50;
            }

            msIndicator.ResetIndicator();
        }








    }
    System.Collections.IEnumerator DestroyDestructable()
    {
        explosion.SetActive(true);
        yield return new WaitForSeconds(2.8f);
        transform.position = new Vector2(-3, 2);
        explosion.SetActive(false);


        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;

        }
    }

    void UpdateHighScore()
    {
        if (score >= highScore)
        {
            highScore = score;

            PlayerPrefs.SetInt("HighScore", highScore);
        }

        highscoreText.text = "Highscore: " + highScore;
    }
}
