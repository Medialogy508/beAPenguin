using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Masks : MonoBehaviour
{
    GameObject current;
    int id;

    Masks(int _id)
    {
        current = this.gameObject;

        id = _id;

    }

    void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }
}
