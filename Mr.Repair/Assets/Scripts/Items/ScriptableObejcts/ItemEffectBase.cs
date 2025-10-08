using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffectBase : ScriptableObject
{
    public abstract void ExecuteEffect(GameObject target);
}
