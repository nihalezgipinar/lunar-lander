using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MSIndicator : MonoBehaviour
{
    public float speed = 1.0f;

    public bool IsReceivingPlayerInput;

    bool isPaused;

    public float lowestValue = -200;
    public float highestValue = 0;
    public float greenValue = -100;

    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, highestValue, 0);
    }

    private void Update()
    {
        if (isPaused) { return; }



        var yPosition = transform.localPosition.y;

        if (IsReceivingPlayerInput)
        {
            if (yPosition < highestValue )
            {
                yPosition += Time.deltaTime * speed;
            }
        }
        else
        {
            if (yPosition >= lowestValue)
            {
                yPosition -= Time.deltaTime * speed;
            }
        }

        transform.localPosition = new Vector3(transform.localPosition.x, yPosition, 0);
    }

    public bool CheckIfInGreen()
    {
        isPaused = true;

        if (transform.localPosition.y > greenValue - 10 && transform.localPosition.y < greenValue + 10)
        {
            return true;
        }

        return false;
    }

    public void ResetIndicator()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, highestValue, 0);

        isPaused = false;
    }
}
