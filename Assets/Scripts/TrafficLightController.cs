using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public Animator animator;
    public int state = 2; // 0=Red, 1=Yellow, 2=Green

    private float redDuration;
    private float yellowDuration;
    private float greenDuration;

    public bool IsGreen => state == 2;

    void Start()
    {
        if (!CompareTag("RailLight"))
        {
            redDuration = 3f;
            yellowDuration = 2f;
            greenDuration = Random.Range(4f, 6f);
        }
        else
        {
            redDuration = Random.Range(4f, 6f);
            yellowDuration = 0f;
            greenDuration = 4f;
        }
        

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        StartCoroutine(CycleTrafficLights());
    }

    IEnumerator CycleTrafficLights()
    {
        while (true)
        {
            state = 0; // Red
            animator.SetInteger("State", state);
            yield return new WaitForSeconds(redDuration);

            state = 1; // Yellow
            animator.SetInteger("State", state);
            yield return new WaitForSeconds(yellowDuration);

            state = 2; // Green
            animator.SetInteger("State", state);
            yield return new WaitForSeconds(greenDuration);
        }
    }
}
