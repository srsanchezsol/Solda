using UnityEngine;
using UnityEngine.EventSystems;

public class MobileMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Direction direction;
    public PlayerController player;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (direction)
        {
            case Direction.Up:
                player.MoveUp();
                break;

            case Direction.Down:
                player.MoveDown();
                break;

            case Direction.Left:
                player.MoveLeft();
                break;

            case Direction.Right:
                player.MoveRight();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.StopMove();
    }
}