using Sirenix.OdinInspector;
using UnityEngine;

namespace DancingLineFanmade.Level
{
    [DisallowMultipleComponent]
    public class SuspendGame : MonoBehaviour
    {
        [SerializeField, DrawWithUnity] private KeyCode targetKey = KeyCode.Escape;
        [SerializeField, DrawWithUnity] private KeyCode continueKey = KeyCode.Space;

        void Update()
        {
            if (LevelManager.GameState == GameStatus.Playing)
            {
                if (Input.GetKeyDown(targetKey))
                {
                    AudioManager.Pitch = 0f;
                    Time.timeScale = 0f;
                    LevelManager.GameState = GameStatus.Suspended;
                    Debug.Log("Game Suspended");
                }
                if (LevelManager.GameState == GameStatus.Suspended && Input.GetKeyDown(continueKey))
                {
                    AudioManager.Pitch = 1f;
                    Time.timeScale = Player.Instance.levelData.timeScale;
                    LevelManager.GameState = GameStatus.Playing;
                    Debug.Log("Game Continued");
                }
            }

        }
    }

}

