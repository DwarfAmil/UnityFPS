using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerHUD _playerHUD;

    // 소지중인 무기 4종류
    [SerializeField] private WeaponBase[] _weapons;

    // 현재 사용중인 무기
    private WeaponBase _currentWeapon;
    
    // 직전에 사용했던 무기
    private WeaponBase _previousWeapon;

    private void Awake()
    {
        // 무기 정보 출력을 위한 현재 소지중인 모든 무기 이벤트 등록
        _playerHUD.SetupAllWeapons(_weapons);

        // 현재 소지중인 모든 무기를 보이지 않게 설정
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i].gameObject != null)
            {
                _weapons[i].gameObject.SetActive(false);
            }
        }

        // Main 무기를 현재 사용 무기로 설정
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;

        var inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        // 교체 가능 무기가 없으면 종료
        if (_weapons[(int)weaponType] == null) return;

        // 현재 사용중인 무기가 있으면 이전 무기 정보 저장
        if (_currentWeapon != null)
        {
            _previousWeapon = _currentWeapon;
        }

        // 무기 교체
        _currentWeapon = _weapons[(int)weaponType];

        // 현재 사용중인 무기로 교체하려고 할 때 종료
        if (_currentWeapon == _previousWeapon) return;
        
        // 무기를 사용하는 PlayerController, PlayerHUD에 현재 무기 정보 전달
        _playerController.SwitchingWeapon(_currentWeapon);
        _playerHUD.SwitchingWeapon(_currentWeapon);

        // 이전에 사용하던 무기 비활성화
        if (_previousWeapon != null)
        {
            _previousWeapon.gameObject.SetActive(false);
        }
        
        // 현재 사용하는 무기 활성화
        _currentWeapon.gameObject.SetActive(true);
    }
}
