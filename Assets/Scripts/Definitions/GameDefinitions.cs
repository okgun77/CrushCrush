using UnityEngine;

// 오브젝트 관련
public enum EObjectType 
{ 
    NONE = -1,
    BASIC,          
    EXPLOSIVE,      
    INDESTRUCTIBLE,
    LENGTH
}

// 점수 관련
public enum EScoreType 
{ 
    NONE = -1,
    TYPE_A,
    TYPE_B,
    TYPE_C,
    LENGTH 
}

// 상태 관련
public enum EGameState
{
    NONE = -1,
    IDLE,
    MOVE,
    ATTACK,
    DEAD,
    LENGTH
}

// 오디오 관련
public enum EAudioType
{
    NONE = -1,
    BGM,
    SFX,
    UI,
    LENGTH
}

// 아이템 관련
public enum EItemType
{
    NONE = -1,
    HEALTH,
    POWER,
    SPEED,
    LENGTH
}

// UI 관련
public enum EUIState
{
    NONE = -1,
    MAIN_MENU,
    GAME_PLAY,
    PAUSE,
    GAME_OVER,
    LENGTH
} 