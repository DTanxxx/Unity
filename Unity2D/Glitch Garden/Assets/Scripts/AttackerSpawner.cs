using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour
{
    [SerializeField] float minSpawnDelay = 1f;
    [SerializeField] float maxSpawnDelay = 5f;
    [SerializeField] Attacker[] attackerPrefabArray;

    bool spawn = true;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (spawn)
        {          
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            if (!spawn)
            {
                break;
            }
            SpawnAttacker();
        }
    }

    public void StopSpawning()
    {
        spawn = false;
    }

    private void SpawnAttacker()
    {
        var attackerIndex = Random.Range(0, attackerPrefabArray.Length);
        Spawn(attackerPrefabArray[attackerIndex]);
    }

    private void Spawn(Attacker myAttacker)
    {
        Attacker newAttacker = Instantiate(
                               myAttacker,
                               transform.position,
                               Quaternion.identity) as Attacker;
        // set the parent object of newAttacker to this spawner
        newAttacker.transform.parent = transform;
    }

    public int NumberOfAttackerChildren()
    {
        return transform.childCount;
    }

}
