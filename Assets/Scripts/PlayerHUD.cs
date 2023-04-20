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

    private void Awake()
    {
        SetupWeapon();
        
        // 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의 Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        _weapon._onAmmoEvent.AddListener(UpdateAmmoHUD);
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
}
