using UnityEngine;

public class ChangeNPC : MonoBehaviour
{
    public static ChangeNPC instance;
    public GameObject NPCBeforeKey;
    public GameObject NPCAfterKey;
    public GameObject WallBlocked;
    public SpriteRenderer sprite;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SoundManagerScript.soundManagerScript.PlaySFX(SoundManagerScript.soundManagerScript.collectable);
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            NPCBeforeKey.SetActive(false);
            NPCAfterKey.SetActive(true);
            sprite.sprite = null;
            //this.gameObject.SetActive(false);
        }
    }
    public void DisableWall()
    {
        WallBlocked.SetActive(false);
    }
}
