using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class MecanicaImage : MonoBehaviour
{
    public GameObject UIHead, UIBody, UILegs;
    public GameObject UIHead2, UIBody2, UILegs2;
    public GameObject[] Robots;
    private Dictionary<string, int> pieceIdentifiers;
    private Dictionary<string, RobotStats> pieceStats; // Diccionario para las estadísticas de las piezas
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

        pieceStats = new Dictionary<string, RobotStats>
        {
            { "head-0", new RobotStats { attack = 10, defense = 0 } },
            { "head-1", new RobotStats { attack = 0, defense = 10 } },
            { "head-2", new RobotStats { attack = 5, defense = 5 } },
            { "body-0", new RobotStats { attack = 0, defense = 10 } },
            { "body-1", new RobotStats { attack = 0, defense = 5 } },
            { "body-2", new RobotStats { attack = 5, defense = 5 } },
            { "legs-0", new RobotStats { attack = 0, defense = 2 } },
            { "legs-1", new RobotStats { attack = 3, defense = 0 } },
            { "legs-2", new RobotStats { attack = 5, defense = 5 } }
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

            tapToPlace.currentRobotStats.attack = pieceStats["head-" + headID] != null ? pieceStats["head-" + headID].attack : 0;
            tapToPlace.currentRobotStats.defense = pieceStats["body-" + bodyID] != null ? pieceStats["body-" + bodyID].defense : 0;
            tapToPlace.currentRobotStats.health = 100; // Valor base de salud
            tapToPlace.currentRobotStats.attackRange = 5; // Valor base de rango de ataque
            tapToPlace.currentRobotStats.moveSpeed = 3; // Valor base de velocidad de movimiento

            // Ajusta las estadísticas según las piezas
            tapToPlace.currentRobotStats.attack += pieceStats["head-" + headID].attack + pieceStats["body-" + bodyID].attack;
            tapToPlace.currentRobotStats.defense += pieceStats["body-" + bodyID].defense + pieceStats["legs-" + legsID].defense;

            tapToPlace.currentRobotStats = totalStats; // Asigna las estadísticas al prefab
            tapToPlace.isReadyToPlace = true;

            tapToPlace.currentRobotStats.attack = 0;
            tapToPlace.currentRobotStats.defense = 0;


        }
    }

    public void ResetParts()
    {
        headID = bodyID = legsID = -1;
        if (UIHead != null) UIHead.GetComponent<Image>().sprite = null;
        if (UIBody != null) UIBody.GetComponent<Image>().sprite = null;
        if (UILegs != null) UILegs.GetComponent<Image>().sprite = null;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage addedImage in args.added)
        {
            ActivateTrackedObject(addedImage.referenceImage.name, addedImage);
        }
    }
}
