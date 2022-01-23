using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WebcamPreview : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var device = WebCamTexture.devices.FirstOrDefault(d => d.name.Contains("5000"));
        Debug.Log(device.name);
        var texture = new WebCamTexture(device.name, 960, 544, 30);
        texture.name = device.name;
        Debug.Log(texture);

        var image = GetComponent<RawImage>();
        image.texture = texture;
        texture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
