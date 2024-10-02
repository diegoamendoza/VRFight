using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class MecanicaImage : MonoBehaviour
{
    public GameObject UIHead, UIBody, UILegs; // Referencias a los objetos UI donde se mostrarán las partes
    public GameObject[] Robots; // Array con los 6 robots diferentes
    private Dictionary<string, int> pieceIdentifiers; // Diccionario para identificar cada pieza
    private int headID = -1, bodyID = -1, legsID = -1; // Identificadores para las piezas escaneadas

    private ARTrackedImageManager arTrackedImageManager;

    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();

        // Inicializar el diccionario de piezas
        pieceIdentifiers = new Dictionary<string, int>
        {
            { "head-a", 0 }, { "head-b", 1 },
            { "body-a", 0 }, { "body-b", 1 },
            { "legs-a", 0 }, { "legs-b", 1 }
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

        // Convertir la textura de la imagen de referencia en un Sprite
        Sprite trackedSprite = Sprite.Create(referenceImageTexture,
                                             new Rect(0, 0, referenceImageTexture.width, referenceImageTexture.height),
                                             new Vector2(0.5f, 0.5f));

        // Asigna la imagen a la UI según el tipo
        if (imageName.Contains("head"))
        {
            if (UIHead != null)
            {
                UIHead.GetComponent<Image>().sprite = trackedSprite;
            }
            headID = pieceIdentifiers[imageName]; // Asigna el ID de la cabeza escaneada
        }
        else if (imageName.Contains("body"))
        {
            if (UIBody != null)
            {
                UIBody.GetComponent<Image>().sprite = trackedSprite;
            }
            bodyID = pieceIdentifiers[imageName]; // Asigna el ID del cuerpo escaneado
        }
        else if (imageName.Contains("legs"))
        {
            if (UILegs != null)
            {
                UILegs.GetComponent<Image>().sprite = trackedSprite;
            }
            legsID = pieceIdentifiers[imageName]; // Asigna el ID de las piernas escaneadas
        }

        // Seleccionar robot cuando se hayan escaneado todas las piezas
        if (headID != -1 && bodyID != -1 && legsID != -1)
        {
            SelectRobot();
        }
    }

    void SelectRobot()
    {
        // Combinar los IDs de las piezas escaneadas para crear un índice único
        int robotIndex = (headID * 4) + (bodyID * 2) + legsID;

        if (robotIndex >= 0 && robotIndex < Robots.Length)
        {
            Debug.Log("Robot seleccionado: " + robotIndex);
            GetComponent<TapToPlace>().prefab = Robots[robotIndex];
        }
        else
        {
            Debug.LogError("Índice de robot fuera de rango: " + robotIndex);
        }
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage addedImage in args.added)
        {
            ActivateTrackedObject(addedImage.referenceImage.name, addedImage);
        }
    }
}
