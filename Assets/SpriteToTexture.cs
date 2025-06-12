using UnityEngine;

public class SpriteToTexture : MonoBehaviour
{
    public Sprite mySprite;

    void Start()
    {
        Texture2D tex = Texture2DScript.SpriteToTexture2D(mySprite);
        // You can now use this texture (e.g., save it, modify it, assign to materials, etc.)
    }
}
