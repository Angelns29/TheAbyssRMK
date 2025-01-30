using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private bool isActive= false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _sprite.sprite = Resources.Load<Sprite>("Checkpoint/CheckpointOn");
            isActive = true;
            PlayerLife.instance.SetCheckpoint(this.transform);

        }
    }
    public bool CheckCheckpoint()
    {
        return isActive;
    }
}
