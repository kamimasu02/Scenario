using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVDataStore : MonoBehaviour
{}

public class BaseData
{
    public const int ALPHA_MAX_VALUE = 100;
    public const int COLOR_MAX_VALUE = 255;
    public const int VOLUME_MAX_VALUE = 100;
    public readonly Color DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);

    public int id;

    public Color ParseColor(string color)
    {
        string[] colorRGBA = color.Split('.');

        if(colorRGBA.Length != 3 && colorRGBA.Length != 4)
        {
            Debug.LogError("Color 인자 에러");
        }

        float colorR = string.IsNullOrWhiteSpace(colorRGBA[0]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[0]) / COLOR_MAX_VALUE);
        float colorG = string.IsNullOrWhiteSpace(colorRGBA[1]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[1]) / COLOR_MAX_VALUE);
        float colorB = string.IsNullOrWhiteSpace(colorRGBA[2]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[2]) / COLOR_MAX_VALUE);

        if(colorRGBA.Length == 3)
        {
            return new Color(colorR,colorG,colorB);   
        }

        float colorA = string.IsNullOrWhiteSpace(colorRGBA[3]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[3]) / ALPHA_MAX_VALUE);

        return new Color(colorR, colorG, colorB, colorA);
    }
}

// 오디어 데이터
public class AudioData : BaseData
{
    public int index;
    public string name;
    public bool loop;
    public float volume;
    public string effect;
    public float effectDurationTime;
    public float startAt;
    public float endAt;
    public float delay;

    public AudioData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        index = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        name = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        loop = string.IsNullOrWhiteSpace(data[3]) ? false : true;
        volume = string.IsNullOrWhiteSpace(data[4]) ? 1.0f : Mathf.Clamp01((float) Convert.ToInt16(data[4]) / VOLUME_MAX_VALUE);
        effect = string.IsNullOrWhiteSpace(data[5]) ? "" : data[5];
        effectDurationTime = string.IsNullOrWhiteSpace(data[6]) ? 0 : float.Parse(data[6]);
        startAt = string.IsNullOrWhiteSpace(data[7]) ? 0 : float.Parse(data[7]);
        endAt = string.IsNullOrWhiteSpace(data[8]) ? 0 : float.Parse(data[8]);
        delay = string.IsNullOrWhiteSpace(data[9]) ? 0 : float.Parse(data[9]);
    }
}

// 대화 데이터
public class DialogueData : BaseData
{
    public string name;
    public string text;
    public float typeSpeed;
    public string fontAsset;
    public string fontStyle;
    public int fontSize;
    public Color fontColor;
    public int voiceIndex;
    public string tag;

    public DialogueData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        name = string.IsNullOrWhiteSpace(data[1]) ? "" : data[1];
        text = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        typeSpeed = string.IsNullOrWhiteSpace(data[3]) ? 0 : float.Parse(data[3]);
        fontAsset = string.IsNullOrWhiteSpace(data[4]) ? "" : data[4];
        fontStyle = string.IsNullOrWhiteSpace(data[5]) ? "" : data[5];
        fontSize = string.IsNullOrWhiteSpace(data[6]) ? -1 : Convert.ToInt16(data[6]);
        fontColor = string.IsNullOrWhiteSpace(data[7]) ? DEFAULT_COLOR : ParseColor(data[7]);
        voiceIndex = string.IsNullOrWhiteSpace(data[8]) ? -1 : Convert.ToInt16(data[8]);
        tag = string.IsNullOrWhiteSpace(data[9]) ? "" : data[9];
    }
}

// 선택지 데이터
public class ChoiceData : BaseData
{
    public int groupId;
    public string text;
    public string scriptFilePathWithName;

    public ChoiceData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        groupId = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        text = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        scriptFilePathWithName = string.IsNullOrWhiteSpace(data[3]) ? "" : data[3];
    }
}

// 캐릭터 초기 세팅 데이터
public class CharacterData : BaseData
{
    public string name;
    public int index;
    public float posX;
    public float posY;
    public float scaleX;
    public float scaleY;
    public bool isActive;
    public string tag;

    public CharacterData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        name = string.IsNullOrWhiteSpace(data[1]) ? "" : data[1];
        index = string.IsNullOrWhiteSpace(data[2]) ? 0 : Convert.ToInt16(data[2]);
        posX = string.IsNullOrWhiteSpace(data[3]) ? Single.NaN : float.Parse(data[3]);
        posY = string.IsNullOrWhiteSpace(data[4]) ? Single.NaN : float.Parse(data[4]);
        scaleX = string.IsNullOrWhiteSpace(data[5]) ? 1 : float.Parse(data[5]);
        scaleY = string.IsNullOrWhiteSpace(data[6]) ? 1 : float.Parse(data[6]);
        isActive = string.IsNullOrWhiteSpace(data[7]) ? false : true;
        tag = string.IsNullOrWhiteSpace(data[8]) ? "" : data[8];
    }
}

// 이미지 데이터
public class ImageData : BaseData
{
    public int index;
    public string name;
    public float posX;
    public float posY;
    public float posChangeTime;
    public float scaleX;
    public float scaleY;
    public float scaleChangeTime;
    public Color? color;
    public float colorChangeTime;
    public string effect;
    public float effectDurationTime;
    public float alphaValue;
    public float blurValue;

    public ImageData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        index = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        name = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        posX = string.IsNullOrWhiteSpace(data[3]) ? Single.NaN : float.Parse(data[3]);
        posY = string.IsNullOrWhiteSpace(data[4]) ? Single.NaN : float.Parse(data[4]);
        posChangeTime = string.IsNullOrWhiteSpace(data[5]) ? 0 : float.Parse(data[5]);
        scaleX = string.IsNullOrWhiteSpace(data[6]) ? Single.NaN : float.Parse(data[6]);
        scaleY = string.IsNullOrWhiteSpace(data[7]) ? Single.NaN : float.Parse(data[7]);
        scaleChangeTime = string.IsNullOrWhiteSpace(data[8]) ? 0 : float.Parse(data[8]);
        color = string.IsNullOrWhiteSpace(data[9]) ? null : ParseColor(data[9]);
        colorChangeTime = string.IsNullOrWhiteSpace(data[10]) ? 0 : float.Parse(data[10]);
        effect = string.IsNullOrWhiteSpace(data[11]) ? "" : data[11];
        effectDurationTime = string.IsNullOrWhiteSpace(data[12]) ? 0 : float.Parse(data[12]);
        alphaValue = string.IsNullOrWhiteSpace(data[13]) ? -1 : float.Parse(data[13]);
        blurValue = string.IsNullOrWhiteSpace(data[14]) ? 0 : float.Parse(data[14]);
    }
}

// 캐릭터 스프라이트 데이터
public class SpriteData : BaseData
{
    public int groupId;
    public int index;
    public string name;
    public float posX;
    public float posY;
    public float posChangeTime;
    public float scaleX;
    public float scaleY;
    public float scaleChangeTime;
    public Color? color;
    public float colorChangeTime;
    public string effect;
    public float effectDurationTime;
    public float alphaValue;
    public float blurValue;
    public bool flip;
    public string animationState;
    public int iconId;
    public string tag;

    public SpriteData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        groupId = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        index = string.IsNullOrWhiteSpace(data[2]) ? -1 : Convert.ToInt16(data[2]);
        name = string.IsNullOrWhiteSpace(data[3]) ? "" : data[3];
        posX = string.IsNullOrWhiteSpace(data[4]) ? Single.NaN : float.Parse(data[4]);
        posY = string.IsNullOrWhiteSpace(data[5]) ? Single.NaN : float.Parse(data[5]);
        posChangeTime = string.IsNullOrWhiteSpace(data[6]) ? 0 : float.Parse(data[6]);
        scaleX = string.IsNullOrWhiteSpace(data[7]) ? Single.NaN : float.Parse(data[7]);
        scaleY = string.IsNullOrWhiteSpace(data[8]) ? Single.NaN : float.Parse(data[8]);
        scaleChangeTime = string.IsNullOrWhiteSpace(data[9]) ? 0 : float.Parse(data[9]);
        color = string.IsNullOrWhiteSpace(data[10]) ? null : ParseColor(data[10]);
        colorChangeTime = string.IsNullOrWhiteSpace(data[11]) ? 0 : float.Parse(data[11]);
        effect = string.IsNullOrWhiteSpace(data[12]) ? "" : data[12];
        effectDurationTime = string.IsNullOrWhiteSpace(data[13]) ? 0 : float.Parse(data[13]);
        alphaValue = string.IsNullOrWhiteSpace(data[14]) ? -1 : float.Parse(data[14]);
        blurValue = string.IsNullOrWhiteSpace(data[15]) ? -1 : float.Parse(data[15]);
        flip = string.IsNullOrWhiteSpace(data[16]) ? false : true;
        animationState = string.IsNullOrWhiteSpace(data[17]) ? "" : data[17];
        iconId = string.IsNullOrWhiteSpace(data[18]) ? -1 : Convert.ToInt16(data[18]);
        tag = string.IsNullOrWhiteSpace(data[19]) ? "" : data[19];
    }
}

// 캐릭터 아이콘 데이터
public class IconData : BaseData
{
    public int index;
    public string name;
    public float posX;
    public float posY;
    public float posChangeTime;
    public float scaleX;
    public float scaleY;
    public float scaleChangeTime;
    public Color color;
    public float colorChangeTime;
    public string effect;
    public float effectDurationTime;
    public float alphaValue;
    public float blurValue;
    public bool flip;

    public IconData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        index = string.IsNullOrWhiteSpace(data[1]) ? 0 : Convert.ToInt16(data[1]);
        name = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        posX = string.IsNullOrWhiteSpace(data[3]) ? Single.NaN : float.Parse(data[3]);
        posY = string.IsNullOrWhiteSpace(data[4]) ? Single.NaN : float.Parse(data[4]);
        posChangeTime = string.IsNullOrWhiteSpace(data[5]) ? 0 : float.Parse(data[5]);
        scaleX = string.IsNullOrWhiteSpace(data[6]) ? -1 : float.Parse(data[6]);
        scaleY = string.IsNullOrWhiteSpace(data[7]) ? -1 : float.Parse(data[7]);
        scaleChangeTime = string.IsNullOrWhiteSpace(data[8]) ? 0 : float.Parse(data[8]);
        color = string.IsNullOrWhiteSpace(data[9]) ? DEFAULT_COLOR : ParseColor(data[9]);
        colorChangeTime = string.IsNullOrWhiteSpace(data[10]) ? 0 : float.Parse(data[10]);
        effect = string.IsNullOrWhiteSpace(data[11]) ? "" : data[11];
        effectDurationTime = string.IsNullOrWhiteSpace(data[12]) ? 0 : float.Parse(data[12]);
        alphaValue = string.IsNullOrWhiteSpace(data[13]) ? 0 : float.Parse(data[13]);
        blurValue = string.IsNullOrWhiteSpace(data[14]) ? 0 : float.Parse(data[14]);
        flip = string.IsNullOrWhiteSpace(data[15]) ? false : true;
    }
}

// 스크립트 데이터
public class ScriptData : BaseData
{
    public int dialogueId;
    public int choiceId;
    public int spriteId;
    public int itemId;
    public int backgroundId;
    public int foregroundId;
    public int sceneId;
    public int audioBGMId;
    public float audioBGMDelay;
    public int audioSFXId;
    public float audioSFXDelay;
    public bool autoPass;
    public bool blockInteraction;

    public ScriptData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        dialogueId = string.IsNullOrWhiteSpace(data[1]) ? 0 : Convert.ToInt16(data[1]);
        choiceId = string.IsNullOrWhiteSpace(data[2]) ? -1 : Convert.ToInt16(data[2]);
        spriteId = string.IsNullOrWhiteSpace(data[3]) ? -1 : Convert.ToInt16(data[3]);
        itemId = string.IsNullOrWhiteSpace(data[4]) ? -1 : Convert.ToInt16(data[4]);
        backgroundId = string.IsNullOrWhiteSpace(data[5]) ? -1 : Convert.ToInt16(data[5]);
        foregroundId = string.IsNullOrWhiteSpace(data[6]) ? -1 : Convert.ToInt16(data[6]);
        sceneId = string.IsNullOrWhiteSpace(data[7]) ? -1 : Convert.ToInt16(data[7]);
        audioBGMId = string.IsNullOrWhiteSpace(data[8]) ? -1 : Convert.ToInt16(data[8]);
        audioSFXId = string.IsNullOrWhiteSpace(data[9]) ? -1 : Convert.ToInt16(data[9]);
        autoPass = string.IsNullOrWhiteSpace(data[10]) ? false : true;
        blockInteraction = string.IsNullOrWhiteSpace(data[11]) ? false : true;
    }
}