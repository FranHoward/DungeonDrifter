using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(Health))]
public class EnemySound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAudioEmitter emitter;

    [Header("Clips")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Variation")]
    [SerializeField, Range(0f, 0.3f)] private float pitchJitter = 0.08f;

    private EnemyAI enemyAI;
    private Health health;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        enemyAI.OnAttack += HandleAttack;
        health.OnDamaged += HandleDamaged;
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        enemyAI.OnAttack -= HandleAttack;
        health.OnDamaged -= HandleDamaged;
        health.OnDeath -= HandleDeath;
    }

    private void HandleAttack() => Play(attackSound);

    private void HandleDamaged(float amount)
    {
        if (!health.IsDead)
            Play(hurtSound);
    }

    private void HandleDeath()
    {
        if (emitter != null)
            emitter.PlayDeathAndRelease(deathSound, GetRandomPitch());
    }

    private void Play(AudioClip clip)
    {
        if (emitter != null)
            emitter.PlayOneShot(clip, GetRandomPitch());
    }

    private float GetRandomPitch()
    {
        return Random.Range(1f - pitchJitter, 1f + pitchJitter);
    }
}
