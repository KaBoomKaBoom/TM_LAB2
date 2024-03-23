using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
   
   public void Random()
   {
    
    SceneManager.LoadScene("MainScene");

   }
	public void Custom()
	{

		SceneManager.LoadScene("CustomGen");

	}
}
