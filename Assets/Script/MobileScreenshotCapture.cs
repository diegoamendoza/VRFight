using UnityEngine;
using System.IO;

public class MobileScreenshotCapture : MonoBehaviour
{
    public void ScreenShotButton()
    {
        StartCoroutine(TakeScreenshot());
    }

    private System.Collections.IEnumerator TakeScreenshot()
    {
        // Esperar al final del cuadro para asegurarse de que la imagen esté renderizada
        yield return new WaitForEndOfFrame();

        // Crear un Texture2D con el tamaño de la pantalla
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshotTexture.Apply();

        // Convertir la imagen a formato PNG
        byte[] imageBytes = screenshotTexture.EncodeToPNG();

        // Liberar la memoria de la textura
        Destroy(screenshotTexture);

        // Guardar la imagen en la carpeta de imágenes del dispositivo Android
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, imageBytes);

        Debug.Log("Screenshot saved to: " + path);
    }
}
