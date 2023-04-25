using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    // 현재 정보가 출력되는 무기
    [SerializeField] private WeaponAssaultRifle _weapon;
    
    // 플레이어의 상태 (이동 속도, 체력)
    [SerializeField] private Status _status;

    [Header("Weapon Base")]
    // 무기 이름
    [SerializeField] private TextMeshProUGUI _textWeaponName;

    // 무기 아이콘
    [SerializeField] private Image _imageWeaponIcon;

    // 무기 아이콘에 사용되는 sprite 배열
    [SerializeField] private Sprite[] _spriteWeaponIcons;

    [Header("Ammo")]
    // 현재 / 최대 탄 수 출력 Text
    [SerializeField] private TextMeshProUGUI _textAmmo;

    [Header("Magazine")]
    // 탄창 UI 프리팹
    [SerializeField] private GameObject _magazineUIPrefab;
    
    // 탄창 UI가 배치되는 Panel
    [SerializeField] private Transform _magazineParent;
    
    // 탄창 UI 리스트
    private List<GameObject> _magazineList;

    [Header("HP & BloodScreen UI")]
    // 플레이어의 체력을 출력하는 Text
    [SerializeField] private TextMeshProUGUI _textHP;

    // 플레이어가 공격 받았을 때 화면에 표기되는 Image
    [SerializeField] private Image _imageBloodScreen;
    
    [SerializeField] private AnimationCurve _curveBloodScreen;

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();
        
        // 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의 Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        _weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        _weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
        _status.onHPEvent.AddListener(UpdateHPHUD);
    }

    private void SetupWeapon()
    {
        _textWeaponName.text = _weapon.WeaponName.ToString();
        _imageWeaponIcon.sprite = _spriteWeaponIcons[(int)_weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int curruntAmmo, int maxAmmo)
    {
        _textAmmo.text = $"<size=40>{curruntAmmo}/</size>{maxAmmo}";
    }

    private void SetupMagazine()
    {
        // _weapon에 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
        // _magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화 / 리스트에 저장
        _magazineList = new List<GameObject>();

        for (int i = 0; i < _weapon.MaxMagazine; i++)
        {
            GameObject clone = Instantiate(_magazineUIPrefab);
            clone.transform.SetParent(_magazineParent);
            clone.SetActive(false);
            
            _magazineList.Add(clone);
        }
        
        // _weapon에 등록되어 있는 현재 탄창 개수만큼 오브젝트 활성화
        for (int i = 0; i < _weapon.CurrentMagazine; i++)
        {
            _magazineList[i].SetActive(true);
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        // 전부 비활성화하고, currentMagazine 개수만큼 활성화
        for (int i = 0; i < _magazineList.Count; i++)
        {
            _magazineList[i].SetActive(false);
        }

        for (int i = 0; i < currentMagazine; i++)
        {
            _magazineList[i].SetActive(true);
        }
    }

    private void UpdateHPHUD(int previous, int current)
    {
        _textHP.text = "HP " + current;

        if (previous - current > 0)
        {
            StopCoroutine(nameof(OnBloodScreen));
            StartCoroutine(nameof(OnBloodScreen));
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = _imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, _curveBloodScreen.Evaluate(percent));
            _imageBloodScreen.color = color;

            yield return null;
        }
    }
}
