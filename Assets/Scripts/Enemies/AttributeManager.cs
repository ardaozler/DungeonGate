using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{

        public int health;
        public int attack;
    public int magicresistance;
   
    public void TakeDamage(int amount)
    {
        health -= amount - (amount * magicresistance / 100); ;
    }

    public void DealDamage(GameObject target)
    {
        var atm = target.GetComponent<AttributeManager>();
        if(atm != null)
        {
            atm.TakeDamage(attack);

        }



    }

    }
