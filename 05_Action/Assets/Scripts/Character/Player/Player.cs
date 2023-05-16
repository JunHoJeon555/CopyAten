using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player : MonoBehaviour, IHealth, IMana, IEquipTarget, IBattle
{
    /// <summary>
    /// 이 플레이어가 가지고 있을 인벤토리
    /// </summary>
    Inventory inven;

    public Inventory Inventory => inven;    // 반쯤 테스트용도

    /// <summary>
    /// 플레이어가 가지고 있는 돈
    /// </summary>
    int money = 0;
    public int Money
    {
        get => money;
        set
        {
            if(money != value)
            {
                money = value;
                //Debug.Log($"Money : {money}");
                onMoneyChange?.Invoke(money);
            }
        }
    }

    /// <summary>
    /// 생존 여부 표시용 변수
    /// </summary>
    bool isAlive = true;
    public bool IsAlive => isAlive;

    /// <summary>
    /// 플레이어의 현재 HP
    /// </summary>
    float hp = 100.0f;
    public float HP 
    { 
        get => hp; 
        set
        {
            if (IsAlive)
            {
                hp = value;
                if (hp < 0) // 살아있을 때만 죽게 만들기
                {
                    Die();
                }
                hp = Mathf.Clamp(hp, 0, maxHP);
                onHealthChange?.Invoke(hp / maxHP);
            }
        }
    }

    /// <summary>
    /// HP가 변경되었을 때 실행될 델리게이트
    /// </summary>
    public Action<float> onHealthChange { get; set; }

    /// <summary>
    /// 플레이어가 죽었을 때 실행될 델리게이트
    /// </summary>
    public Action onDie { get; set; }

    /// <summary>
    /// 플레이어의 최대 HP
    /// </summary>
    float maxHP = 100.0f;
    public float MaxHP => maxHP;

    /// <summary>
    /// 플레이어의 현재 MP
    /// </summary>
    float mp = 100.0f;
    public float MP 
    { 
        get => mp; 
        set
        {
            if(IsAlive)
            {
                mp = Mathf.Clamp(value, 0, maxMP);
                onManaChange?.Invoke(mp / maxMP);
            }
        }
    }

    /// <summary>
    /// 플레이어의 최대 MP
    /// </summary>
    float maxMP = 100.0f;
    public float MaxMP => maxMP;

    /// <summary>
    /// 마나가 변경되었을 때 호출될 델리게이트
    /// </summary>
    public Action<float> onManaChange { get; set; }

    /// <summary>
    /// 무기가 장비될 트랜스폼
    /// </summary>
    public Transform weaponParent;

    /// <summary>
    /// 방패가 장비될 트랜스폼
    /// </summary>
    public Transform shieldParent;

    /// <summary>
    /// 장비 아이템의 부위별 장비 상태(장비한 아이템이 있는 슬롯을 가지고 있음)
    /// </summary>
    ItemSlot[] partsSlots;

    /// <summary>
    /// 장비 아이템의 부위별 슬롯 확인용 인덱서(null이면 장비 안되어있음)
    /// </summary>
    public ItemSlot this[EquipType part] => partsSlots[(int)part];

    /// <summary>
    /// 돈이 변경되었을 때 실행될 델리게이트
    /// </summary>
    public Action<int> onMoneyChange;

    /// <summary>
    /// 아이템을 줏을 수 있는 거리
    /// </summary>
    public float ItemPickupRange = 2.0f;

    /// <summary>
    /// 플레이어의 현재 속도 비율(0이면 정지, 1이면 최고 속도)
    /// </summary>
    public float SpeedRatio => playerController.SpeedRatio;

    public float attackPower = 10.0f;
    public float AttackPower => attackPower;

    public float defencePower = 3.0f;
    public float DefencePower => defencePower;

    /// <summary>
    /// 무기 활성화 비활성화를 알리는 델리게이트. 파라메터가 true면 켜는 것, false면 꺼지는 것
    /// </summary>
    Action<bool> onWeaponEnable;

    /// <summary>
    /// 무기 이펙트 활성화 비활성화를 알리는 델리게이트. 파라메터가 true면 켜는 것, false면 꺼지는 것
    /// </summary>
    Action<bool> onWeaponEffectEnable;

    PlayerController playerController;

    Animator anim;

    CinemachineVirtualCamera dieCamera;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        playerController = GetComponent<PlayerController>();
        playerController.onItemPickUp = OnItemPickUp;   // 아이템 줍는다는 신호가 들어오면 줍는 처리 실행

        dieCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);

        //weaponParent = FindObjectOfType<WeaponPosition>().transform;

        // EquipType 갯수만큼 배열 크기 확보
        partsSlots = new ItemSlot[Enum.GetValues(typeof(EquipType)).Length];    
    }

    private void Start()
    {
        inven = new Inventory(this);    // SceneLoaded보다 나중에 실행되어야 해서 Awake에서 하면 안됨
        if(GameManager.Inst.InvenUI != null)
        {
            GameManager.Inst.InvenUI.InitializeInventory(inven);
        }
    }

    /// <summary>
    /// 아이템 줍는 처리를 하는 함수
    /// </summary>
    private void OnItemPickUp()
    {
        // 일정 범위 안에 있는 아이템 찾고
        Collider[] items = Physics.OverlapSphere(transform.position, ItemPickupRange, LayerMask.GetMask("Item"));
        foreach (Collider itemCollider in items)
        {
            Item item = itemCollider.gameObject.GetComponent<Item>();

            IConsumable consumable = item.ItemData as IConsumable;  // 즉시 소비되는 아이템인지 확인
            if(consumable != null)
            {
                consumable.Consume(gameObject);     // 즉시 소비되는 아이템이면 바로 사용
                Destroy(item.gameObject);           // 사용 후 제거
            }
            else if ( inven.AddItem(item.ItemData.code) ) // 하나씩 인벤토리에 추가하고
            {
                Destroy(item.gameObject);           // 성공하면 먹은 아이템 삭제하기
            }
        }
    }

    /// <summary>
    /// 플레이어 사망 처리용 함수
    /// </summary>
    public void Die()
    {
        isAlive = false;

        anim.SetLayerWeight(1, 0.0f);
        anim.SetTrigger("Die");
        
        dieCamera.gameObject.SetActive(true);        

        onDie?.Invoke();
        Debug.Log("플레이어 사망");
    }

    /// <summary>
    /// 체력을 지속적으로 회복시키는 함수
    /// </summary>
    /// <param name="totalRegen">전체 회복량</param>
    /// <param name="duration">전체 회복하는데 걸리는 시간</param>
    public void HealthRegenerate(float totalRegen, float duration)
    {
        StartCoroutine(HealthGenerateCoroutine(totalRegen, duration));
    }

    IEnumerator HealthGenerateCoroutine(float totalRegen, float duration)
    {
        // 초당 회복량 : totalRegen/duration
        float regenPerSec = totalRegen / duration;
        float timeElapsed = 0.0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            HP += Time.deltaTime * regenPerSec;
            yield return null;
        }
    }

    /// <summary>
    /// 마나를 지속적으로 회복시키는 함수
    /// </summary>
    /// <param name="totalRegen">전체 회복량</param>
    /// <param name="duration">전체 회복하는데 걸리는 시간</param>
    public void ManaRegenerate(float totalRegen, float duration)
    {
        StartCoroutine(ManaGenerateCoroutine(totalRegen, duration));
    }

    IEnumerator ManaGenerateCoroutine(float totalRegen, float duration)
    {
        // 초당 회복량 : totalRegen/duration
        float regenPerSec = totalRegen / duration;
        float timeElapsed = 0.0f;
        while( timeElapsed < duration )
        {
            timeElapsed += Time.deltaTime;
            MP += Time.deltaTime * regenPerSec;
            yield return null;
        }
    }

    /// <summary>
    /// 아이템을 장비하는 함수
    /// </summary>
    /// <param name="part">아이템을 장비할 부위</param>
    /// <param name="slot">장비할 아이템이 들어있는 슬롯</param>
    public void EquipItem(EquipType part, ItemSlot slot)
    {
        ItemData_Equip equip = slot.ItemData as ItemData_Equip;
        if(equip!= null)
        {
            Transform partParent = GetPartTransform(part);
            GameObject obj = Instantiate(equip.equipPrefab, partParent);  // 생성하고
            partsSlots[(int)part] = slot;                           // 기록해 놓기
            slot.IsEquipped = true;                                 // 장비되었다고 알림

            if(part == EquipType.Weapon)
            {
                Weapon weapon = obj.GetComponent<Weapon>();
                onWeaponEnable = weapon.ColliderEnable;
                onWeaponEffectEnable = weapon.EffectPlay;
            }
        }
    }

    /// <summary>
    /// 아이템 장비를 해제하는 함수
    /// </summary>
    /// <param name="part">아이템을 해제할 부위</param>
    public void UnEquipItem(EquipType part)
    {
        Transform partParent = GetPartTransform(part);
        while(partParent.childCount > 0)                    // 손에 붙어있는 것 모두 제거
        {
            Transform child = partParent.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        if (part == EquipType.Weapon)
        {
            onWeaponEnable = null;
            onWeaponEffectEnable = null;
        }

        partsSlots[(int)part].IsEquipped = false;           // 장비 해제되었다고 알림
        partsSlots[(int)part] = null;                       // 기록 비워두기
    }

    /// <summary>
    /// 장비아이템이 붙을 부모 트랜스폼 찾아주는 함수
    /// </summary>
    /// <param name="part">장비아이템의 종류</param>
    /// <returns>장비 아이템이 붙을 부모 트랜스폼</returns>
    public Transform GetPartTransform(EquipType part)
    {
        Transform result = null;
        switch(part)
        {
            case EquipType.Weapon:
                result = weaponParent;
                break;
            case EquipType.Shield: 
                result = shieldParent;
                break;
        }
        return result;
    }

    /// <summary>
    /// 무기 컬러이더 활성화하라고 신호 보내는 함수.(애니메이션에서 실행 시킴)
    /// </summary>
    public void WeaponEnable()
    {
        onWeaponEnable?.Invoke(true);
    }

    /// <summary>
    /// 무기 컬러이더 비활성화하라고 신호 보내는 함수.(애니메이션에서 실행 시킴)
    /// </summary>
    public void WeaponDisable()
    {
        onWeaponEnable?.Invoke(false);
    }

    /// <summary>
    /// 무기 이팩트 활성화/비활성화하라고 신호보내는 함수.(AttackState 계열 애니메이션에서 실행 시킴)
    /// </summary>
    /// <param name="enable">true면 활성화, false면 비활성화</param>
    public void WeaponEffectEnable(bool enable)
    {
        onWeaponEffectEnable?.Invoke(enable);
    }

    /// <summary>
    /// 무기와 방패를 표시하거나 표시하지 않는 함수
    /// </summary>
    /// <param name="isShow">true면 보여주고, false면 안보여주기</param>
    public void ShowWeaponAndShield(bool isShow)
    {
        weaponParent.gameObject.SetActive(isShow);
        shieldParent.gameObject.SetActive(isShow);
    }

    public void Attack(IBattle target)
    {
        target?.Defence(AttackPower);
        Debug.Log($"{target.transform.gameObject.name}에게 {attackPower}만큼 공격을 했습니다.");
    }

    public void Defence(float damage)
    {
        if (IsAlive)
        {
            anim.SetTrigger("Hit");
            HP -= Mathf.Max(1, damage - DefencePower);
            Debug.Log($"{Mathf.Max(1, damage - DefencePower)}만큼의 데미지를 입었습니다.");
        }
    }

    /// <summary>
    /// 테스트용
    /// </summary>
    /// <param name="code">추가할 아이템 종류</param>
    /// <param name="count">추가할 갯수</param>
    public void Test_AddItem(ItemCode code, uint count = 1)
    {
        for(int i = 0; i < count; i++)
        {
            inven.AddItem(code);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;

        // 아이템 획득 반경 그리기
        Handles.DrawWireDisc(transform.position, Vector3.up, ItemPickupRange);        
    }

#endif
}
