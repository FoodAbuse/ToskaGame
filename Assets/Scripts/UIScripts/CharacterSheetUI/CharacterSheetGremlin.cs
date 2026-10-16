using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetGremlin : UIReporter
{
    
    // character sheet will have these things

    // equipped weapon. this will be linked to an interactable slot
    
    private GameObject _characterSheet;
    public GameObject CharacterSheet { get { return _characterSheet; } }
    public GameObject CharacterSheetPanel;
    public WeaponRetrieverSlot weaponSlot;
    public ClothingSlot clothingSlot;
    
    
    private Vector2 _screenOffset;

    public Vector2 ScreenOffset
    {
        get { return _screenOffset; }
        set
        {
            switch (value.x)
            {
                case > 1:
                    _screenOffset.x = 1;
                    break;
                case < -1:
                    _screenOffset.x = -1;
                    break;
                default:
                    _screenOffset.x = value.x;
                    break;
            }

            switch (value.y)
            {
                case > 1:
                    _screenOffset.y = 1;
                    break;
                case < -1:
                    _screenOffset.y = -1;
                    break;
                default:
                    _screenOffset.y = value.y;
                    break;
            }
        }
    }
    // equipped headgear
    // other character stuff

    public void UpdateUI()
    {
        // this method looks at the data of the attached slots and tells them to update to match the ui
        // liek the inventory gremlin
    }
    //[TODO] give UIGremlin a Parent class that this can call on instead. or an interface
    public void StartGremlin(UIReporter targetToFollow)
    {
        Vector3 targetPosition = new Vector2(targetToFollow.RootTarget.transform.position.x + targetToFollow.RootTarget.rect.size.x /2, targetToFollow.RootTarget.transform.position.y);
        RectTransform csRect = CharacterSheetPanel.GetComponent<RectTransform>();
        csRect.pivot = new Vector2(0, .5f);
        csRect.anchoredPosition = targetPosition;

        //[TODO] write code for handling displaying on Left/right of target

    }

    public void StartGremlin(Vector2 newPanelOffset)
    {
        RectTransform ipRect = CharacterSheetPanel.GetComponent<RectTransform>();
        Rect areaRect = Screen.safeArea;
        // perform calculation to return screen offset scaled into the Safe area -1 far left 0 mid, 1 max
        Vector2 targetPosition;
        targetPosition.x = ((ScreenOffset.x - -1) * (areaRect.xMax - areaRect.xMin)) / 2 + 0;
        targetPosition.y = ((ScreenOffset.y - -1) * (areaRect.yMax - areaRect.yMin)) / 2;
        
        float RectHalfwidth = (ipRect.rect.width*ipRect.localScale.x/2);
        float RectHalfheight = ipRect.rect.height*ipRect.localScale.y/2;
        if(targetPosition.x - RectHalfwidth < areaRect.xMin)
        {
            
            targetPosition.x += (RectHalfwidth - targetPosition.x);
        }

        if (targetPosition.y - RectHalfheight < areaRect.yMin)
        {
            targetPosition.y += (RectHalfheight - targetPosition.y);
        }

        if (targetPosition.x + RectHalfwidth > areaRect.xMax)
        {
            targetPosition.x -= (RectHalfheight - ( areaRect.xMax- targetPosition.x));
        }

        if (targetPosition.y + RectHalfheight > areaRect.yMax)
        {
            targetPosition.y -= (RectHalfheight- (areaRect.yMax - targetPosition.y));
        }
        
        ipRect.anchorMax = new Vector2(0, 0);
        ipRect.anchorMin = new Vector2(0, 0);
        ipRect.pivot = new Vector2(.5f,.5f);
        ipRect.anchoredPosition = targetPosition;
        //[TODO] Write code for creating a floating Charactersheet not latched onto any other panel
    }
    
    
}

public class CharacterSheetData : ScriptableObject
{
    // this will just be a Singletopn that keeps track of what the character has equipped
    private static CharacterSheetData _playerData;

    public static CharacterSheetData PlayerData
    {
        get
        {
            if (_playerData == null)
            {
                _playerData = ScriptableObject.CreateInstance<CharacterSheetData>();
            }

            return _playerData;
        }

    }

    public WeaponData equippedWeapon;
    public ClothingData equippedClothing;
}
