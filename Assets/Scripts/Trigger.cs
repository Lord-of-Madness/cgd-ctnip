using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    //public event EventHandler OnEnterTrigger;
    public UnityEvent<Trigger, PlayerController> OnEnterTriggerWithCollider;
    public bool OnlyOnce;
    bool happened = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player)) { 
            if(OnlyOnce && happened) return; //only once
            happened = true;
            OnEnterTriggerWithCollider.Invoke(this, player);
        }
        //OnEnterTrigger.Invoke(this,new());
    }
}
