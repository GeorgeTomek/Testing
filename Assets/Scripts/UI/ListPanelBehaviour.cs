using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPanelBehaviour : MonoBehaviour {

    //TODO: Pop up animation
	void Start () {
        transform.localScale = Vector3.zero;
        StartCoroutine(PopUpAnimation());
	}

    private IEnumerator PopUpAnimation()
    {
        var wait = new WaitForEndOfFrame();
        var scale = new Vector3(1.3f, 1.2f, 1f);
        while (transform.localScale.x <= 1.18f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * 3f);
            yield return wait;
        }
        scale = new Vector3(1f, 1f, 1f);
        while (transform.localScale.x >= 1.01f)
        {
            transform.localScale = Vector3.Slerp(transform.localScale, scale, Time.deltaTime * 4f);
            yield return wait;
        }


    }
}
