using UnityEngine;

public class ElementProjectile : MonoBehaviour
{
    public ElementData elementData;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has an IElementReceiver component
        IElementReceiver receiver = collision.gameObject.GetComponent<IElementReceiver>();
        if (receiver != null)
        {
            // Receive the incoming element
            receiver.ReceiveElement(elementData, transform.position);
        }

        // Destroy projectile
        Destroy(this.gameObject);
    }
}
