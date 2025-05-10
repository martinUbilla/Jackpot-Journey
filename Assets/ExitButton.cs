using UnityEngine;

public class ExitButton : MonoBehaviour
{
   public void QuitApplication()
    {
        Debug.Log("salio");
        Application.Quit();
    }
}
