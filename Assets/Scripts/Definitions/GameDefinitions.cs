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
    NORMAL,
    BOSS_1,
    BOSS_2,
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

public enum EEffectType
{
    NONE = -1,
    BREAK,      // 부수기
    EXPLOSION,  // 폭발
    HIT,        // 피격
    SPAWN,      // 생성
    LENGTH
}