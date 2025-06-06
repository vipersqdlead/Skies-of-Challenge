using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{

    public Image blackBG;
    public bool ActivateFadeIn, ActivateFadeOut;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ActivateFadeIn)
        {
            FadeIn();
        }

        if (ActivateFadeOut)
        {
            FadeOut();
        }
    }

    public void FadeIn()
    {
        blackBG.fillOrigin = 1;
        blackBG.fillAmount -= Time.unscaledDeltaTime * 2f;
        if(blackBG.fillAmount == 0)
        {
            ActivateFadeIn = false;
        }
    }

    public void FadeOut()
    {
        blackBG.fillOrigin = 2;
        blackBG.fillAmount += Time.unscaledDeltaTime * 2f;
        if( blackBG.fillAmount == 1)
        {
            ActivateFadeOut = false;
        }
    }

   /* public IEnumerator Fading(bool inOrOut)
    {
        if (inOrOut)
        {
            blackBG.fillOrigin = 1;
            {
                blackBG.fillAmount -= Time.deltaTime * 2f;
                yield return new WaitForSeconds(2f);
            }
        }

        if (!inOrOut)
        {
            blackBG.fillOrigin = 2;
            blackBG.fillAmount += Time.deltaTime * 2f;
            yield return new WaitForSeconds(2f);
        }


    }*/
}
