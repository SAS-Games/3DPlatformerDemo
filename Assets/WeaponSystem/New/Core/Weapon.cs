using UnityEngine;

namespace SAS.WeaponSystem
{
    public class Weapon : MonoBehaviour, IWeapon
    {
        public float AttackStartTime { get; internal set; }
        public WeaponDataSO Data { get; private set; }

        private int currentAttackCounter;
        public int CurrentAttackCounter
        {
            get => currentAttackCounter;
            private set => currentAttackCounter = value >= Data.NumberOfAttacks ? 0 : value;
        }

        public bool CanEnterAttack { get; set; }

        void IWeapon.Init()
        {
            throw new System.NotImplementedException();
        }

        void IWeapon.Enter()
        {
            Debug.Log("Enter");
        }

        void IWeapon.Exit()
        {
            throw new System.NotImplementedException();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetData(WeaponDataSO data)
        {
            Data = data;

            if (Data is null)
                return;

            CurrentAttackCounter = 0;
        }


    }
}
