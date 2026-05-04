using GameManagerScripts;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Get().SwitchGameState(GameState.GameWin);

    }
}
