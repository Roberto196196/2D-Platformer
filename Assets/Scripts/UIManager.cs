
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioSource deathSoundEffect;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject tutorialScreen;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Time.timeScale = 0f;
            tutorialScreen.SetActive(true);
        }
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(pauseMenu.activeSelf)
            {
                PauseGame(false);
            }
            else
            {
                PauseGame(true);
            }
        }
    }
    
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        deathSoundEffect.Play();
    }
    
    public void Tutorial()
    {
        Time.timeScale = 1f;
        tutorialScreen.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void PauseGame(bool status)
    {
        pauseMenu.SetActive(status);
        
        if (status)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    
    public void SoundVolume(float volume)
    {
        SoundManager.instance.ChangeSoundVolume(volume);
    }
    
    public void MusicVolume(float volume)
    {
        SoundManager.instance.ChangeMusicVolume(volume);
    }
}
