using UnityEngine;

public class CharacterLogic : MonoBehaviour
{
    GameObject iceTrapInstance;
    bool isFrozen;
    public void Freeze()
    {
        if (isFrozen)
            return;
        iceTrapInstance = Instantiate(CharacterManager.instance.iceTrapPrefab, this.transform);
        Destroy(iceTrapInstance, CharacterManager.instance.freezeDuration);
        isFrozen = true;
    }
}