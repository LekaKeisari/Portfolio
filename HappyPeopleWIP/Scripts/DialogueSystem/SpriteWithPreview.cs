using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class SpriteWithPreview
{
    //This is a custom class that allows the preview of sprites in editor

    [ShowAssetPreview]
    public Sprite sprite;
}
