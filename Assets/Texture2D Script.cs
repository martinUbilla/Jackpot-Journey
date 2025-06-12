using UnityEngine;

public class Texture2DScript 
{
    public static Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width || sprite.rect.height != sprite.texture.height)
        {
            // The sprite is a sub-region of the texture (i.e., part of an atlas)
            Texture2D newTex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }
        else
        {
            // The sprite uses the whole texture
            return sprite.texture;
        }
    }
}
