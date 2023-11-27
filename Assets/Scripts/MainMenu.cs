using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public AudioSource mainMenuMusic;
   private float _musicVolume = 1f;
   
   private void Start()
   {
      mainMenuMusic.Play();
   }
   private void Update()
   {
      mainMenuMusic.volume = _musicVolume;
   }
   public void PlayGame()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   }

   public void QuitGame()
   {
      Debug.Log("QUIT!");
      Application.Quit();
   }
   
   public void SetVolume( float volume)
   {
      _musicVolume = volume;
   }
}
