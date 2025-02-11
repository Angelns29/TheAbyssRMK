using UnityEngine;

public class CollectableLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManagerScript.soundManagerScript.PlaySFX(SoundManagerScript.soundManagerScript.collectable);
            GameManager.gameManager.SetCollectableTrue(gameObject.name);
            GameManager.gameManager.ShowCollectablesCount();
            gameObject.SetActive(false);
        }
    }
}
