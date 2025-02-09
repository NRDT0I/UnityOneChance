using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 2f;
    public LayerMask ladderLayer;
    private bool isOnLadder = false;
    private bool isClimbing = false;
    private Transform ladderTransform;
    private MovementScript movementScript;
    public Image fadeScreen;
    public float fadeDuration = 0.5f;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        if (fadeScreen == null)
        {
            Debug.LogError("fadeScreen не привязан! Убедись, что Image есть в Canvas.");
        }
    }

    void Update()
    {
        if (isOnLadder && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TeleportPlayer());
        }

        if (isClimbing)
        {
            HandleClimbing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((ladderLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            isOnLadder = true;
            ladderTransform = other.transform;
            Debug.Log("Игрок у лестницы.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((ladderLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            isOnLadder = false;
            isClimbing = false;
            movementScript.enabled = true;
            ladderTransform = null;
            Debug.Log("Игрок ушел от лестницы.");
        }
    }

    private IEnumerator TeleportPlayer()
    {
        if (ladderTransform == null)
        {
            Debug.LogWarning("ladderTransform = null! Игрок не в зоне лестницы.");
            yield break;
        }

        movementScript.enabled = false;
        yield return StartCoroutine(FadeToBlack());

        Transform ladderTop = ladderTransform.Find("LadderTop");
        Transform ladderBottom = ladderTransform.Find("LadderBottom");

        if (ladderTop == null || ladderBottom == null)
        {
            Debug.LogError("Ошибка! У лестницы нет LadderTop или LadderBottom.");
            yield break;
        }

        Vector3 targetPosition;
        Vector3 forwardDirection = ladderTransform.forward;

        if (transform.position.y > (ladderTop.position.y + ladderBottom.position.y) / 2)
        {
            targetPosition = ladderBottom.position - forwardDirection * 1.5f;
            Debug.Log("Телепортируем вниз.");
        }
        else
        {
            targetPosition = ladderTop.position + forwardDirection * 1.5f;
            Debug.Log("Телепортируем вверх.");
        }

        transform.position = targetPosition;

        yield return StartCoroutine(FadeFromBlack());

        movementScript.enabled = true;
    }

    private IEnumerator FadeToBlack()
    {
        Debug.Log("Начинаем затемнение экрана.");
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadeScreen.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeScreen.color = new Color(0, 0, 0, 1);
        Debug.Log("Экран полностью затемнен.");
    }

    private IEnumerator FadeFromBlack()
    {
        Debug.Log("Начинаем плавное появление экрана.");
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeScreen.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeScreen.color = new Color(0, 0, 0, 0);
        Debug.Log("Экран полностью виден.");
    }

    private void HandleClimbing()
    {
        float verticalMove = Input.GetAxis("Vertical");
        Vector3 climbDirection = Vector3.up * verticalMove * climbSpeed;
        transform.Translate(climbDirection * Time.deltaTime);
    }
}
