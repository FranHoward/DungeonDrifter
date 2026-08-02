using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [Serializable]
    private struct WeaponSound
    {
        public WeaponData weapon;
        public AudioClip clip;
    }

    [Header("Weapon Sounds")]
    [SerializeField] private WeaponSound[] weaponSounds;

    [Header("Health Sounds")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Pickup Sound")]
    [SerializeField] private AudioClip pickupSound;

    private AudioSource audioSource;
    private Health health;
    private WeaponAttack weaponAttack;
    private PlayerInventory inventory;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        weaponAttack = GetComponent<WeaponAttack>();
        inventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDamaged += HandleDamaged;
            health.OnHealed += HandleHealed;
            health.OnDeath += HandleDeath;
        }

        if (weaponAttack != null)
            weaponAttack.OnAttack += HandleAttack;

        if (inventory != null)
            inventory.OnItemCollected += HandleItemCollected;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDamaged -= HandleDamaged;
            health.OnHealed -= HandleHealed;
            health.OnDeath -= HandleDeath;
        }

        if (weaponAttack != null)
            weaponAttack.OnAttack -= HandleAttack;

        if (inventory != null)
            inventory.OnItemCollected -= HandleItemCollected;
    }

    private void HandleAttack(WeaponData usedWeapon)
    {
        foreach (WeaponSound weaponSound in weaponSounds)
        {
            if (weaponSound.weapon == usedWeapon)
            {
                Play(weaponSound.clip);
                return;
            }
        }
    }

    private void HandleDamaged(float amount)
    {
        if (!health.IsDead)
            Play(hurtSound);
    }

    private void HandleHealed(float amount) => Play(healSound);

    private void HandleDeath() => Play(deathSound);

    private void HandleItemCollected() => Play(pickupSound);

    private void Play(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
