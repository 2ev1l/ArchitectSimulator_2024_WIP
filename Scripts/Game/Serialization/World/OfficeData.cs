using Game.DataBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class OfficeData : RentablePremiseData
    {
        #region fields & properties
        public UnityAction<int> OnEmployeeRemoved;
        public UnityAction<int> OnEmployeeAdded;
        /// <summary>
        /// O(1) instead of List.Count
        /// </summary>
        public int CurrentEmployeesCount
        {
            get
            {
                FixCurrentEmployeesCount();
                return currentEmployeesCount;
            }
            set
            {
                FixCurrentEmployeesCount();
                currentEmployeesCount = value;
            }
        }
        public IReadOnlyList<int> CurrentEmployees => currentEmployees;
        [SerializeField] private List<int> currentEmployees = new(); //todo
        [System.NonSerialized] private int currentEmployeesCount = -1;
        #endregion fields & properties

        #region methods
        private void FixCurrentEmployeesCount()
        {
            if (currentEmployeesCount >= 0) return;
            currentEmployeesCount = currentEmployees.Count;
        }
        public bool CanAddEmployee(int employeeReference)
        {
            int maxEmployees = ((OfficeInfo)Info).MaximumEmployees;
            if (maxEmployees <= CurrentEmployeesCount) return false;
            return true;
        }
        public bool TryAddEmployee(int employeeReference)
        {
            if (!CanAddEmployee(employeeReference)) return false;
            currentEmployees.Add(employeeReference);
            CurrentEmployeesCount--;
            return true;
        }
        public void RemoveEmployee(int employeeReference)
        {
            currentEmployees.Remove(employeeReference);
            CurrentEmployeesCount++;
            OnEmployeeRemoved?.Invoke(employeeReference);
        }
        protected override void OnInfoReplaced()
        {
            base.OnInfoReplaced();
        }
        public override bool CanReplaceInfo(int newInfoId)
        {
            OfficeInfo newOffice = (OfficeInfo)DB.Instance.RentableOfficeInfo.Find(x => x.Data.PremiseInfo.Id == newInfoId).Data.PremiseInfo;
            return (newOffice.MaximumEmployees - CurrentEmployeesCount) >= 0;
        }
        protected override RentablePremise GetRentablePremiseInfo() => DB.Instance.RentableOfficeInfo.Find(x => x.Data.PremiseInfo.Id == Id).Data;
        public OfficeData(int id) : base(id) { }
        #endregion methods
    }
}