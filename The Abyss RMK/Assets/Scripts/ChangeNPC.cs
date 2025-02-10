using UnityEngine;

public class ChangeNPC : MonoBehaviour
{
    public static ChangeNPC instance;
    public GameObject NPCBeforeKey;
    public GameObject NPCAfterKey;
    public GameObject WallBlocked;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NPCBeforeKey.SetActive(false);
            NPCAfterKey.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    public void DisableWall()
    {
        WallBlocked.SetActive(false);
    }
}
