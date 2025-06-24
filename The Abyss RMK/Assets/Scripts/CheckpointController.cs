using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private bool isActive= false;
    public GameObject SpriteMinimap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _sprite.sprite = Resources.Load<Sprite>("Checkpoint/CheckpointOn");
            isActive = true;
            PlayerLife.instance.SetCheckpoint(this.transform);
            ShowIcon();

        }
    }
    public bool CheckCheckpoint()
    {
        return isActive;
    }
    public void ShowIcon()
    {
        SpriteMinimap.SetActive(true);
    }
}
