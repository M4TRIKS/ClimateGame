using UnityEngine;
using System.Collections;
using System.Threading;

public class MenuController: MonoBehaviour
{
    public CanvasGroup mainMenu;

    public CanvasGroup optionsMenu;

    public float transitionDuration = .25f;

    void Start()
    {
        ShowMainMenuInstant();
    }

    public void OpenOptionsMenu()
    {
        StartCoroutine(TransitionMenus(mainMenu,optionsMenu));

    }

public void BackTot()
    {
        StartCoroutine(TransitionMenus(optionsMenu, mainMenu));
        
    }
    void ShowMainMenuInstant()
    {
        mainMenu.alpha  = 1; //alpha is opacity
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true; //straight lin (used for shooters a lot) //this time is the raycast from the mouse

        optionsMenu.alpha = 0;
        optionsMenu.interactable = false;
        optionsMenu.blocksRaycasts = false;
        optionsMenu.gameObject.SetActive(false); // hicghcase game object is the component, lower case is?

    }


    IEnumerator TransitionMenus(CanvasGroup currentMenu, CanvasGroup nextMenu)
    
    {
        float timer = 0f;
        nextMenu.gameObject.SetActive(true);
        while(timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer/transitionDuration;
            currentMenu.alpha = 1 - progress;
            nextMenu.alpha = progress;
            yield return null;

        }

        currentMenu.interactable = false;
        currentMenu.blocksRaycasts =false;
        currentMenu.gameObject.SetActive(false);
        nextMenu.interactable =true ;
        nextMenu.blocksRaycasts = true ;

    }


}
