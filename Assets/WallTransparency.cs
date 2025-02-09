using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallTransparency : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleLayer;
    public float transparencyAmount = 0.3f;
    public float fadeDuration = 0.5f;
    private Dictionary<Renderer, Coroutine> fadeCoroutines = new Dictionary<Renderer, Coroutine>();
    private HashSet<Renderer> currentTransparentObjects = new HashSet<Renderer>();

    void Update()
    {
        HashSet<Renderer> newTransparentObjects = new HashSet<Renderer>();
        Vector3 direction = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, obstacleLayer);

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                StartFade(rend, transparencyAmount);
                newTransparentObjects.Add(rend);
            }
        }

        foreach (var rend in currentTransparentObjects)
        {
            if (!newTransparentObjects.Contains(rend))
            {
                StartFade(rend, 1f);
            }
        }
        
        currentTransparentObjects = newTransparentObjects;
    }

    private void StartFade(Renderer rend, float targetAlpha)
    {
        if (fadeCoroutines.ContainsKey(rend))
        {
            StopCoroutine(fadeCoroutines[rend]);
            fadeCoroutines.Remove(rend);
        }
        Coroutine fadeCoroutine = StartCoroutine(FadeObject(rend, targetAlpha));
        fadeCoroutines[rend] = fadeCoroutine;
    }

    private IEnumerator FadeObject(Renderer rend, float targetAlpha)
    {
        foreach (Material mat in rend.materials)
        {
            if (!mat.HasProperty("_Color"))
                continue;
            
            Color color = mat.color;
            float startAlpha = color.a;
            float elapsedTime = 0f;
            
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                color.a = newAlpha;
                mat.color = color;
                yield return null;
            }

            color.a = targetAlpha;
            mat.color = color;
        }
        
        fadeCoroutines.Remove(rend);
    }
}
