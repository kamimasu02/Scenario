using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private RectTransform _rt;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _sprites;

    private int _index = 0;

    private float _posX;
    private float _posY;
    private float _scaleX;
    private float _scaleY;

    private bool _isSpriteChanged = false;

    void LateUpdate()
    {
        UpdateObjectTransform();
    }

    private void UpdateObjectTransform()
    {
        if(_index == 0)
        {
            _spriteRenderer.sprite = null;
        }
        else if(_index > 0)
        {
            if(_isSpriteChanged)
            {
                _spriteRenderer.sprite = _sprites[_index - 1];
                _isSpriteChanged = false;
            }
        }

        _rt.anchoredPosition = new Vector2(_posX, _posY);
        _rt.localScale = new Vector2(_scaleX, _scaleY);
    }

    public void SetSpriteIndex(int index)
    {
        if (index < 0 && index >= _sprites.Length)
        {
            throw new ArgumentException("잘못된 Sprite Index 사용");
        }

        _index = index;
        _isSpriteChanged = true;
    }

    public void SetSpritePosition(float x, float y)
    {
        _posX = x;
        _posY = y;
    }

    public void SetSpriteScale(float x, float y)
    {
        _scaleX = x;
        _scaleY = y;
    }

    public void SetIconData(IconData data)
    {
        SetSpriteIndex(data.index);

        if(data.index > 0)
        {
            SetSpritePosition(data.posX, data.posY);
            SetSpriteScale(data.scaleX, data.scaleY);
        }
    }
}
