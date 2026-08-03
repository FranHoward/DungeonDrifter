using UnityEngine;

public class EnemyHitFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private ParticleSystem hitParticles;

    [Header("Particles")]
    [SerializeField, Min(1)] private int particleCount = 14;
    [SerializeField, Min(0f)] private float particleSpeed = 3.5f;
    [SerializeField, Range(0f, 2f)] private float particleSpread = 0.75f;

    [Header("Screen Shake")]
    [SerializeField, Min(0f)] private float shakeDuration = 0.1f;
    [SerializeField, Min(0f)] private float shakeMagnitude = 0.09f;

    private void Awake()
    {
        ResolveParticleSystem();
        EnableUnscaledSimulation();
        ResolveScreenShake();
    }

    private void OnValidate()
    {
        ResolveParticleSystem();
        EnableUnscaledSimulation();
    }

    public void Play(Vector3 hitPosition, Vector3 impactDirection)
    {
        if (hitParticles != null)
            EmitParticles(hitPosition, impactDirection);

        ResolveScreenShake();
        screenShake?.Play(shakeDuration, shakeMagnitude);
    }

    private void ResolveParticleSystem()
    {
        if (hitParticles == null)
            hitParticles = GetComponentInChildren<ParticleSystem>(true);
    }

    private void EnableUnscaledSimulation()
    {
        if (hitParticles == null)
            return;

        ParticleSystem.MainModule main = hitParticles.main;
        main.useUnscaledTime = true;
    }

    private void EmitParticles(Vector3 hitPosition, Vector3 impactDirection)
    {
        if (impactDirection.sqrMagnitude < 0.001f)
            impactDirection = transform.forward;

        impactDirection.Normalize();

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = Mathf.Abs(randomDirection.y);

            Vector3 direction = (
                impactDirection
                + Vector3.up * 0.35f
                + randomDirection * particleSpread).normalized;

            ParticleSystem.EmitParams particle = new ParticleSystem.EmitParams
            {
                position = hitPosition,
                velocity = direction * Random.Range(particleSpeed * 0.65f, particleSpeed)
            };

            hitParticles.Emit(particle, 1);
        }
    }

    private void ResolveScreenShake()
    {
        if (screenShake != null)
            return;

        screenShake = FindFirstObjectByType<ScreenShake>();

        if (screenShake == null && Camera.main != null)
            screenShake = Camera.main.gameObject.AddComponent<ScreenShake>();
    }
}
