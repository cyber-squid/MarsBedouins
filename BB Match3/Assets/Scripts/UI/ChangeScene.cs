using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ChangeScene : MonoBehaviour
{
    //AudioMenu am;

    public GameObject options;

    //private void Awake()
    //{
    //    am = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioMenu>();

    //}
    public void ChangeSceneScript()
    {

        if (options != null)
        {             
            options.GetComponent<OptionManager>().Resume();
        }
        SceneManager.LoadScene("Levels");
 
    }

    //public void changeScene()
    //{
    //    StartCoroutine(waiting());
    //}
    public void LoadSceneScript(string sceneName)
    {
        //StartCoroutine(waiting());
        if (options != null)
        {
            options.GetComponent<OptionManager>().Resume();
        }
        SceneManager.LoadScene(sceneName);
        
    }

    //private IEnumerator waiting()
    //{
    //    am.PlaySFX(am.select);
    //    yield return new WaitForSeconds(0.5f);


    //}
}
