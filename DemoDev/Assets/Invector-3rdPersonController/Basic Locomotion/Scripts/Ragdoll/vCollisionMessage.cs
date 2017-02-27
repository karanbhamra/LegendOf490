using UnityEngine;
using System.Collections;
using Invector.CharacterController;
public class vCollisionMessage : MonoBehaviour 
{	
    [HideInInspector]
	public vRagdoll ragdoll;  

    void Start()
    {
        ragdoll = GetComponentInParent<vRagdoll>();
    }

	void OnCollisionEnter(Collision collision)
	{
        if (collision != null )
		{           
            if(ragdoll)
            {
                ragdoll.OnRagdollCollisionEnter(new vRagdollCollision(this.gameObject, collision));
                if (!inAddDamage)
                {
                    float impactforce = collision.relativeVelocity.x + collision.relativeVelocity.y + collision.relativeVelocity.z;
                    if (impactforce > 10 || impactforce < -10)
                    {
                        inAddDamage = true;
                        Damage damage = new Damage((int)Mathf.Abs(impactforce) - 10);
                        damage.ignoreDefense = true;
                        damage.sender = collision.transform;
                        damage.hitPosition = collision.contacts[0].point;
                        ragdoll.ApplyDamage(damage);
                        Invoke("ResetAddDamage", 0.1f);
                    }
                }
                
            }           
		}
	}
    bool inAddDamage;
    /// <summary>
    /// Send Damage to Character root every 0.1 seconds. See the <see cref="vRagdoll.ApplyDamage(Damage)"/>
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(Damage damage)
    {   
        if(!inAddDamage)
        {
            inAddDamage = true;
            ragdoll.ApplyDamage(damage);
            Invoke("ResetAddDamage", 0.1f);
        } 
    }
    void ResetAddDamage()
    {
        inAddDamage = false;
    }
 
}
