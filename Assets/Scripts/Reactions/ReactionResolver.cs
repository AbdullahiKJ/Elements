using System.Collections.Generic;
using UnityEngine;

public class ReactionResolver : MonoBehaviour
{
    [SerializeField] private ReactionDatabase database;

    private Dictionary<ReactionKey, ReactionResult> lookup;

    private void Awake()
    {
        BuildLookupTable();
    }

    private void BuildLookupTable()
    {
        lookup = new Dictionary<ReactionKey, ReactionResult>();

        foreach (var entry in database.reactions)
        {
            var key = new ReactionKey
            {
                element = entry.element,
                status = entry.existingStatus
            };

            if (!lookup.ContainsKey(key))
            {
                lookup.Add(key, entry.result);
            }
            else
            {
                Debug.LogWarning($"Duplicate reaction: {entry.element} + {entry.existingStatus}");
            }
        }
    }

    public bool TryGetReaction(
        ElementType element,
        StatusType status,
        out ReactionResult result)
    {
        var key = new ReactionKey
        {
            element = element,
            status = status
        };

        return lookup.TryGetValue(key, out result);
    }
}
