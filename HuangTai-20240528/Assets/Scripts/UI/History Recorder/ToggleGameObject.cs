using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    public GameObject objectToShow; // 要显示的 GameObject
    public GameObject objectToHide; // 要关闭的 GameObject

    public void ToggleObjects()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(true); // 显示 objectToShow
            Debug.Log("Showing objectToShow");
        }

        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // 关闭 objectToHide
            Debug.Log("Showing objectTohide");
        }
    }
}
