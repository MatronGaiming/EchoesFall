using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTargetActivation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Lock player movement.
            //Lerp Camera to a certain position.
            //Activate Leader to walkout side onto the balcony.
            //Lerp Camera back to player.
            //Unlock player movement.
        }
    }
}
