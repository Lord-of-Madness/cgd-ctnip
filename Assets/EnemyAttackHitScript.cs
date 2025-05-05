using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackHitScript : MonoBehaviour
{
    SphereCollider sphereCol;
    BoxCollider boxCol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sphereCol = GetComponent<SphereCollider>();
        boxCol = GetComponent<BoxCollider>();

	}

    // Update is called once per frame
    void Update()
    {

    }

    public List<Collider> GetAllObjectsInAttackArea()
    {
        Collider[] colsInBox = Physics.OverlapBox(transform.position + (transform.rotation * boxCol.center), boxCol.size / 2);
        Collider[] colsInSphere = Physics.OverlapSphere(transform.position + (transform.rotation * sphereCol.center), sphereCol.radius);

        List<Collider> colsInAttackArea = new();
        foreach (Collider col in colsInSphere) {
            bool duplicate = false;
            foreach (Collider col2 in colsInBox)
                if (col2 == col) { duplicate = true; break; }
            if (!duplicate) colsInAttackArea.Add(col);
        }

        return colsInAttackArea;

    }
}
