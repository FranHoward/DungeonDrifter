using UnityEngine;

public class EnemyHitFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private Material particleMaterial;

    [Header("Particles")]
    [SerializeField, Min(1)] private int particleCount = 14;
    [SerializeField, Min(0.01f)] private float particleLifetime = 0.3f;
    [SerializeField, Min(0.01f)] private float particleSize = 0.12f;
    [SerializeField, Min(0f)] private float particleSpeed = 3.5f;
    [SerializeField, Range(0f, 2f)] private float particleSpread = 0.75f;
    [SerializeField] private Color primaryColor = new Color(1f, 0.18f, 0.03f, 1f);
    [SerializeField] private Color secondaryColor = new Color(1f, 0.85f, 0.2f, 1f);

    [Header("Screen Shake")]
    [SerializeField, Min(0f)] private float shakeDuration = 0.1f;
    [SerializeField, Min(0f)] private float shakeMagnitude = 0.09f;

    private ParticleSystem hitParticles;

    private void Awake()
    {
        CreateParticleSystem();
        ResolveScreenShake();
    }

    public void Play(Vector3 hitPosition, Vector3 impactDirection)
    {
        if (hitParticles == null)
            CreateParticleSystem();

        EmitParticles(hitPosition, impactDirection);

        ResolveScreenShake();
        screenShake?.Play(shakeDuration, shakeMagnitude);
    }

    private void CreateParticleSystem()
    {
        GameObject particleObject = new GameObject("Enemy Hit Particles");
        particleObject.transform.SetParent(transform, false);

        hitParticles = particleObject.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = hitParticles.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startLifetime = particleLifetime;
        main.startSpeed = 0f;
        main.startSize = particleSize;
        main.startColor = Color.white;
        main.gravityModifier = 0.75f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = Mathf.Max(128, particleCount * 4);

        ParticleSystem.EmissionModule emission = hitParticles.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = hitParticles.shape;
        shape.enabled = false;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = hitParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fade = new Gradient();
        fade.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.8f, 0.55f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = fade;

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = hitParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

        ParticleSystemRenderer particleRenderer = hitParticles.GetComponent<ParticleSystemRenderer>();
        particleRenderer.renderMode = ParticleSystemRenderMode.Stretch;
        particleRenderer.velocityScale = 0.2f;
        particleRenderer.lengthScale = 2f;
        particleRenderer.sharedMaterial = particleMaterial;

        hitParticles.Play();
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
                velocity = direction * Random.Range(particleSpeed * 0.65f, particleSpeed),
                startLifetime = particleLifetime * Random.Range(0.8f, 1.2f),
                startSize = particleSize * Random.Range(0.7f, 1.25f),
                startColor = Color.Lerp(primaryColor, secondaryColor, Random.value)
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
