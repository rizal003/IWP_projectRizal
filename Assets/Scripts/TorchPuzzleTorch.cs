//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TorchPuzzleTorch : MonoBehaviour
//{
//    public bool isLit = false;
//    private SpriteRenderer sr;
//    public TorchPuzzleManager puzzleManager;

//    void Awake()
//    {
//        sr = GetComponent<SpriteRenderer>();
//        sr.color = Color.black; // Start as "unlit"
//    }

//    public void Light()
//    {
//        if (!isLit)
//        {
//            isLit = true;
//            sr.color = Color.white; // Change to "lit"
//            puzzleManager.OnTorchLit();
//        }
//    }
//}
