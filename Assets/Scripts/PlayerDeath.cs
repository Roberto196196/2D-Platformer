using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    
    private UIManager _uiManager;
    private Animator _anim;
    [SerializeField] private AudioSource _deathSoundEffect;
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }
    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Trap"))
        {
            SoundManager.instance.PlaySound(_deathSoundEffect.clip);
            _anim.Play("Die");
            Time.timeScale = 0f;
            _uiManager.GameOver();
        }
    }
}
