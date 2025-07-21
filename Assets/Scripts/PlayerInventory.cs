using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int keyCount = 0;
    public UnityEvent<int> OnKeyCountChanged;

    public void AddKey()
    {
        keyCount++;
        OnKeyCountChanged?.Invoke(keyCount);
    }

    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            OnKeyCountChanged?.Invoke(keyCount);
            return true;
        }
        return false;
    }
}
