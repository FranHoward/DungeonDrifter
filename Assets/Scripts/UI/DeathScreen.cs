using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Health playerHealth;

    private void OnEnable() => playerHealth.OnDeath += Show;

    private void OnDisable() => playerHealth.OnDeath -= Show;

    private void Awake()
    {
        panel.SetActive(false);
    }
    
    private void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
