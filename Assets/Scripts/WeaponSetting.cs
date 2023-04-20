// 무기의 종류가 여러 종류일 때 공용으로 사용하는 변수들은 구조체로 묶어서 정의
// 변수가 추가 / 삭제될 때 구조체의 선언하기 때문에 추가 / 삭제에 대한 관리가 용이함

public enum WeaponName { AssultRifle = 0 }

// [System.Serializable]를 통해 직렬화를 하지 않으면 다른 클래스의 변수로 생성시 Inspector View에 멤버 변수들의 목록이 뜨지 않음
[System.Serializable]
public struct WeaponSetting
{
    // 무기 이름
    public WeaponName weaponName;

    // 현재 탄약 수
    public int curruntAmmo;

    // 최대 탄약 수
    public int maxAmmo;
    
    // 공격 속도
    public float attackRate;
    
    // 공격 사거리
    public float attackDistance;
    
    // 연속 공격 여부
    public bool isAutomaticAttack;
}