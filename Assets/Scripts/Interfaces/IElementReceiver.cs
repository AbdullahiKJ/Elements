using UnityEngine;

public interface IElementReceiver
{
    void ReceiveElement(ElementData element, Vector3 hitPoint);
}