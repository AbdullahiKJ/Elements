using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReactionDatabase", menuName = "Scriptable Objects/ReactionDatabase")]
public class ReactionDatabase : ScriptableObject
{
    public List<ReactionEntry> reactions = new List<ReactionEntry>();
}
