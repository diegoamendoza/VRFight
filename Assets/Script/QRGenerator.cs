using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QRCoder;
using QRCoder.Unity;

public class QRGenerator : MonoBehaviour
{
   
    void Start()
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode("AJKSD2", QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);
        GameObject.Find("Cube").GetComponent<Renderer>().material.mainTexture = qrCodeAsTexture2D;
    }

    
    void Update()
    {
        
    }
}
