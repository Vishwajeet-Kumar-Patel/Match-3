using UnityEngine;
using System.Collections;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;
    
    [Header("Particle Effects")]
    public ParticleSystem matchEffect;
    public ParticleSystem comboEffect;
    public ParticleSystem tileDestroyEffect;
    public ParticleSystem specialTileEffect;
    
    [Header("Animation Settings")]
    public AnimationCurve scaleCurve;
    public float effectDuration = 0.5f;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void PlayMatchEffect(Vector3 position, int matchCount)
    {
        if (matchEffect != null)
        {
            ParticleSystem effect = Instantiate(matchEffect, position, Quaternion.identity);
            
            // Adjust particle count based on match size
            var emission = effect.emission;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, matchCount * 5)
            });
            
            effect.Play();
            Destroy(effect.gameObject, effectDuration);
        }
    }
    
    public void PlayComboEffect(Vector3 position, int comboLevel)
    {
        if (comboEffect != null)
        {
            ParticleSystem effect = Instantiate(comboEffect, position, Quaternion.identity);
            
            // Scale effect based on combo level
            effect.transform.localScale = Vector3.one * (1f + comboLevel * 0.2f);
            
            effect.Play();
            Destroy(effect.gameObject, effectDuration);
        }
    }
    
    public void PlayTileDestroyEffect(Vector3 position, Color tileColor)
    {
        if (tileDestroyEffect != null)
        {
            ParticleSystem effect = Instantiate(tileDestroyEffect, position, Quaternion.identity);
            
            // Set particle color to match tile
            var main = effect.main;
            main.startColor = tileColor;
            
            effect.Play();
            Destroy(effect.gameObject, effectDuration);
        }
    }
    
    public void PlaySpecialTileEffect(Vector3 position)
    {
        if (specialTileEffect != null)
        {
            ParticleSystem effect = Instantiate(specialTileEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effectDuration);
        }
    }
    
    public void AnimateTileScale(Transform tileTransform, float targetScale = 1.2f)
    {
        StartCoroutine(ScaleAnimation(tileTransform, targetScale));
    }
    
    IEnumerator ScaleAnimation(Transform target, float maxScale)
    {
        if (target == null) yield break;
        
        Vector3 originalScale = target.localScale;
        Vector3 targetScaleVector = originalScale * maxScale;
        
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < effectDuration / 2)
        {
            if (target == null) yield break;
            
            elapsed += Time.deltaTime;
            float progress = elapsed / (effectDuration / 2);
            
            target.localScale = Vector3.Lerp(originalScale, targetScaleVector, scaleCurve.Evaluate(progress));
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale back down
        while (elapsed < effectDuration / 2)
        {
            if (target == null) yield break;
            
            elapsed += Time.deltaTime;
            float progress = elapsed / (effectDuration / 2);
            
            target.localScale = Vector3.Lerp(targetScaleVector, originalScale, scaleCurve.Evaluate(progress));
            yield return null;
        }
        
        if (target != null)
            target.localScale = originalScale;
    }
    
    public void ScreenShake(float intensity = 0.1f, float duration = 0.2f)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            StartCoroutine(ShakeCamera(mainCamera, intensity, duration));
        }
    }
    
    IEnumerator ShakeCamera(Camera camera, float intensity, float duration)
    {
        Vector3 originalPosition = camera.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            camera.transform.position = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        camera.transform.position = originalPosition;
    }
}
