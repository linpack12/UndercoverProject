using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    [SerializeField] private int playerId;
    public int PlayerId => playerId; 

    public static PlayerIdentity LocalPlayer {  get; private set; }

    private void Awake()
    {
        if (LocalPlayer == null)
        {
            LocalPlayer = this; 
        }
    }
}
