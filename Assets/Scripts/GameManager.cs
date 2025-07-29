using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int floorNumber = 1;
    public Vector3? savedPlayerPosition = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextFloor()
    {
        floorNumber++;
        RoomManager.Instance.RegenerateDungeon();

        // Optional: Add visual effects here
        // StartCoroutine(FloorTransitionEffect());
    }

    public void ReturnToTutorial()
    {
        floorNumber = 1;
        RoomManager.Instance.RegenerateDungeon();
    }

   
}