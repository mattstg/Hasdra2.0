using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSpriteStyle
{
    public Color color;
    public GV.SpriteStyle spriteStyle;

    public MaterialSpriteStyle(Color _color, GV.SpriteStyle _spriteStyle)
    {
        color = _color;
        spriteStyle = _spriteStyle;
    }

    public MaterialSpriteStyle(float r, float g, float b, GV.SpriteStyle _spriteStyle, float a = 255)
    {
        color = new Color(r / 255, g / 255, b / 255, a / 255);
        spriteStyle = _spriteStyle;
    }
}
