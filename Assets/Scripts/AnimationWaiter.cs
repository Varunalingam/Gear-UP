using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationWaiter : MonoBehaviour
{
    // Start is called before the first frame update

    Animator A;

    private void Awake()
    {
        A = gameObject.GetComponent<Animator>();
        StartCoroutine(WaitforFrame());
    }

    public IEnumerator WaitforFrame()
    {
        while(!A.GetCurrentAnimatorStateInfo(0).IsTag("End"))
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadSceneAsync(1);
        else
            Application.Quit();
    }

}
