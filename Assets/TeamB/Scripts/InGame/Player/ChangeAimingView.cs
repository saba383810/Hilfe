using UnityEngine;

public class ChangeAimingView : MonoBehaviour
{
   [SerializeField] private SpriteRenderer[] images;
   [SerializeField] private Sprite[] sprites;

   public void Setup(Player.PlayerType playerType)
   {
      foreach (var spriteRenderer in images)
      {
         spriteRenderer.sprite = sprites[(int)playerType];
      }
   }
}
