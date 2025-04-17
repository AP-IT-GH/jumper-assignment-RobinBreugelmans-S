using UnityEngine;

public class Move : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = Vector3.forward * (Random.value + 1) * 4; // 4 - 8
    }

    /*void OnTriggerEnter(Collider other)
    {
        var agent = other.gameObject.GetComponent<JumperAgent>();
        if(agent != null)
        {
            agent.hitObject = true;
        }
    }*/
}
