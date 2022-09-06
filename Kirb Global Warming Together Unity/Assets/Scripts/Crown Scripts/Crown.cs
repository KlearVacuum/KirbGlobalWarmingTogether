using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : ScriptableObject
{
    // Called once kirb gets a crown
    public virtual void OnCrowned(AIEntity kirb) { }

    // Called every frame once kirb gets a crown
    public virtual void CrownedUpdate(AIEntity kirb) { }

    public virtual void CrownedOnDeath(AIEntity kirb) { }
}
