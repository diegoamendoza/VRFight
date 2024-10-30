using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class CardAnimationFeedbacksActivator : MonoBehaviour
{
    public MMF_Player feedbackA, feedbackB, feedbackC;
    public GameObject cardA, cardB, cardC;
    public void ActivateCardAFeedback()
    {
        feedbackA.PlayFeedbacks();

    }

    public void ActivateCardBFeedback()
    {
        feedbackB.PlayFeedbacks();
    }

    public void ActivateCardCFeedback()
    {
        feedbackC.PlayFeedbacks();
    }

    void CheckForBandCactive()
    {
        if(cardB.activeInHierarchy)
        {
            ActivateCardBFeedback();
        }

        if(cardC.activeInHierarchy)
        {
            ActivateCardCFeedback();
        }
    }

    void CheckForAandCactive()
    {
        if (cardA.activeInHierarchy)
        {
            ActivateCardAFeedback();
        }

        if (cardC.activeInHierarchy)
        {
            ActivateCardCFeedback();
        }
    }

    void CheckForAandBactive()
    {
        if (cardA.activeInHierarchy)
        {
            ActivateCardAFeedback();
        }

        if (cardB.activeInHierarchy)
        {
            ActivateCardCFeedback();
        }
    }


}
