using UnityEngine;

public enum CardTypeEnum
{
    ATTACK,
    SUPPORT,
    BUILDING
}

public static class CardTypeEnumEx
{
    public static Color GetColorByType(this CardTypeEnum cType)
    {
        switch (cType)
        {
            case CardTypeEnum.ATTACK:
                return new Color(255, 0, 0);
            case CardTypeEnum.SUPPORT:
                return new Color(0, 0, 255);
            case CardTypeEnum.BUILDING:
                return new Color(0, 255, 0);
        }

        return new Color(0, 0, 0);
    }
}