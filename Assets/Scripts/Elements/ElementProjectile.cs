using UnityEngine;

public class ElementProjectile : MonoBehaviour
{
    public ElementData elementData;

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision, null);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(null, other);
    }

    void HandleCollision(Collision collision, Collider other)
    {
        // Check if the collided/triggered object has an IElementReceiver component
        IElementReceiver receiver;
        if (collision != null)
        {
            receiver = collision.gameObject.GetComponent<IElementReceiver>();
        }
        else if (other != null)
        {
            receiver = other.gameObject.GetComponent<IElementReceiver>();
        }
        else
        {
            receiver = null;
        }

        if (receiver != null)
        {
            // Receive the incoming element
            receiver.ReceiveElement(elementData, transform.position);
        }

        // Destroy projectile
        Destroy(this.gameObject);
    }
}
