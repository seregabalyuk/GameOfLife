using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapesLoader : MonoBehaviour {
    [SerializeField]
    List<Texture2D> textures;
    [SerializeField]
    MouseControler mouseControler;
    
    [SerializeField]
    GameObject scrollContent;
    [SerializeField]
    GameObject iconExample;
    
    // Start is called before the first frame update
    void Start() {
        foreach(var texture in textures) {
            var icon = Instantiate(iconExample, scrollContent.transform);

            var image = icon.GetComponent<Image>();
            image.sprite = Sprite.Create(
                texture,
                new Rect(0, 0,
                    texture.width,
                    texture.height
                ),
                new Vector2(0.5f, 0.5f)
            );

            var rect = icon.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(
                rect.sizeDelta.x,
                rect.sizeDelta.y
            );

            var script = icon.GetComponent<ShapeFromImage>();
            script.SetTexture(texture);
            script.SetMouseController(mouseControler);
            
            var child = icon.transform.GetChild(0);
            var text = child.GetComponent<TextMeshProUGUI>();
            text.text = texture.name + ", cost " + script.GetPoints().Count.ToString();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
