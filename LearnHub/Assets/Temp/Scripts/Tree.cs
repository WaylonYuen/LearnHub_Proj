using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    float Layer = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision) {
        transform.position += new Vector3(0, 0, Layer);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        transform.position += new Vector3(0, 0, -Layer);
    }

}
