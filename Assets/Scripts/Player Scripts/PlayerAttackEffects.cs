using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEffects : MonoBehaviour
{
    public GameObject groundImpactSpawn;
    public GameObject kickSpawn;
    public GameObject fireTornadoSpawn;
    public GameObject fireShieldSpawn;

    public GameObject groundImpactFXPrefab;
    public GameObject kickFXPrefab;
    public GameObject fireTornadoFXPrefab;
    public GameObject fireShieldFXPrefab;
    public GameObject healFXPrefab;
    public GameObject thunderFXPrefab;

    void GroundImpact()
    {
        Instantiate(groundImpactFXPrefab, groundImpactSpawn.transform.position, Quaternion.identity);
    }

    void Kick()
    {
        Instantiate(kickFXPrefab, kickSpawn.transform.position, Quaternion.identity);
    }

    void FireTornado()
    {
        Instantiate(fireTornadoFXPrefab, fireTornadoSpawn.transform.position, Quaternion.identity);
    }

    void FireShield()
    {
        GameObject fire = Instantiate<GameObject>(fireShieldFXPrefab, fireShieldSpawn.transform.position, Quaternion.identity);
        fire.transform.SetParent(transform);
    }

    void Heal()
    {
        Vector3 temp = transform.position;
        temp.y += 2f;
        GameObject heal = Instantiate<GameObject>(healFXPrefab, temp, Quaternion.identity);
        heal.transform.SetParent(transform);
    }

    void ThunderAttack()
    {
        for (int i = 0; i < 8; ++i)
        {
            Vector3 pos = Vector3.zero;
            if (0 == i)
            {
                pos = NewVectorPosition(-4f, 2f);
            }
            else if (1 == i)
            {
                pos = NewVectorPosition(4f, 2f);
            }
            else if (2 == i)
            {
                pos = NewVectorPosition(0f, 2f, -4f);
            }
            else if (3 == i)
            {
                pos = NewVectorPosition(0f, 2f, -4f);
            }
            else if (4 == i)
            {
                pos = NewVectorPosition(2.5f, 2f, 2.5f);
            }
            else if (5 == i)
            {
                pos = NewVectorPosition(-2.5f, 2f, 2.5f);
            }
            else if (6 == i)
            {
                pos = NewVectorPosition(-2.5f, 2f, -2.5f);
            }
            else if (7 == i)
            {
                pos = NewVectorPosition(2.5f, 2f, -2.5f);
            }

            Instantiate(thunderFXPrefab, pos, Quaternion.identity);
        }
    }

    Vector3 NewVectorPosition(float newX = 0f, float newY = 0f, float newZ = 0f)
    {
        return new Vector3(transform.position.x + newX, transform.position.y + newY, transform.position.z + newZ);
    }
}
