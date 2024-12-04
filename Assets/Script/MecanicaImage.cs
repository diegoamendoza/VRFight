using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class MecanicaImage : MonoBehaviour
{
    public GameObject UIHead, UIBody, UILegs,UIObject;
    public GameObject UIHead2, UIBody2, UILegs2, UIObject2;
    public GameObject[] Robots;
    public GameObject[] Objects;
    private Dictionary<string, int> pieceIdentifiers;
    private Dictionary<string, int> objectIdentifiers;

    private Dictionary<int, RobotStats> pieceStats; // Diccionario para las estadísticas de las piezas
    private int headID = -1, bodyID = -1, legsID = -1;
    public Animator cardsUIAnimator;
    public ARTrackedImageManager arTrackedImageManager;
    public TapToPlace tapToPlace;
    public RobotStats totalStats;
    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
        tapToPlace = FindObjectOfType<TapToPlace>();
        totalStats = tapToPlace.gameObject.GetComponent<RobotStats>();

        pieceIdentifiers = new Dictionary<string, int>
        {
            { "head-a", 0 }, { "head-b", 1 },{"head-c",2},
            { "body-a", 0 }, { "body-b", 1 },{"body-c", 2},
            { "legs-a", 0 }, { "legs-b", 1 },{"legs-c",2}
        };

        pieceStats = new Dictionary<int, RobotStats>
        {
            { 0, new RobotStats { attack = 10, defense = 0 } },
            { 1, new RobotStats { attack = 0, defense = 10 } },
            { 2, new RobotStats { attack = 5, defense = 5 } },

        };

        objectIdentifiers = new Dictionary<string, int>
        {
            {"object-0", 0},
            {"object-1", 1},
            {"object-2", 2},
            {"object-3", 3},
        };


    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    void ActivateTrackedObject(string imageName, ARTrackedImage trackedImage)
    {
        Texture2D referenceImageTexture = trackedImage.referenceImage.texture;
        if (referenceImageTexture == null)
        {
            Debug.LogError("The texture of the tracked image is null!");
            return;
        }

        Sprite trackedSprite = Sprite.Create(referenceImageTexture,
                                             new Rect(0, 0, referenceImageTexture.width, referenceImageTexture.height),
                                             new Vector2(0.5f, 0.5f));

        if (imageName.Contains("head"))
        {
            if (UIHead != null)
            {
                UIHead.GetComponent<Image>().sprite = trackedSprite;
                UIHead2.GetComponent<Image>().sprite = trackedSprite;
                cardsUIAnimator.SetTrigger("showCardA");
            }
            headID = pieceIdentifiers[imageName];
        }
        else if (imageName.Contains("body"))
        {
            if (UIBody != null)
            {
                UIBody.GetComponent<Image>().sprite = trackedSprite;
                UIBody2.GetComponent<Image>().sprite = trackedSprite;
                cardsUIAnimator.SetTrigger("showCardB");
            }
            bodyID = pieceIdentifiers[imageName];
        }
        else if (imageName.Contains("legs"))
        {
            if (UILegs != null)
            {
                UILegs.GetComponent<Image>().sprite = trackedSprite;
                UILegs2.GetComponent<Image>().sprite = trackedSprite;
                cardsUIAnimator.SetTrigger("showCardC");
            }
            legsID = pieceIdentifiers[imageName];
        }
        else if (imageName.Contains("object"))
        {
            if(UIObject !=null)
            {
                UIObject.GetComponent<Image>().sprite = trackedSprite;
                UIObject2.GetComponent<Image>().sprite = trackedSprite;
                cardsUIAnimator.SetTrigger("showCardD");
            }
            tapToPlace.prefabObject = Objects[objectIdentifiers[imageName]];
            tapToPlace.objectReady = true;
        }

        if (headID != -1 && bodyID != -1 && legsID != -1)
        {
            SelectRobot();
        }
    }

    void SelectRobot()
    {
        int robotIndex = (headID * 4) + (bodyID * 2) + legsID;
        if (robotIndex >= 0 && robotIndex < Robots.Length)
        {
            Debug.Log("Robot seleccionado: " + robotIndex);
            tapToPlace.prefab = Robots[robotIndex];

            // Calcular las estadísticas totales del robot


            // Ajusta las estadísticas según las piezas
            tapToPlace.currentRobotStats.attack += pieceStats[headID].attack + pieceStats[bodyID].attack + pieceStats[legsID].attack;
            tapToPlace.currentRobotStats.defense += pieceStats[bodyID].defense + pieceStats[legsID].defense + pieceStats[headID].defense;
            tapToPlace.currentRobotStats.health = 100; // Valor base de salud

            tapToPlace.currentRobotStats = totalStats; // Asigna las estadísticas al prefab
            tapToPlace.isReadyToPlace = true;

            

        }
    }

    public void ResetParts()
    {
        headID = bodyID = legsID = -1;
        if (UIHead != null) UIHead.GetComponent<Image>().sprite = null;
        if (UIBody != null) UIBody.GetComponent<Image>().sprite = null;
        if (UILegs != null) UILegs.GetComponent<Image>().sprite = null;
        cardsUIAnimator.SetTrigger("DesactivateCardA");
        cardsUIAnimator.SetTrigger("DesactivateCardB");
        cardsUIAnimator.SetTrigger("DesactivateCardC");
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage addedImage in args.added)
        {
            ActivateTrackedObject(addedImage.referenceImage.name, addedImage);
        }
    }
}
