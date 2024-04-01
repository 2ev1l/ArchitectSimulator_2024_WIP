using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Universal.Collections.Generic;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class CompanyData
    {
        #region fields & properties
        public UnityAction OnCreated;

        public RangedValue Rating => rating;
        [SerializeField] private RangedValue rating = new(0, new(0, 100));
        public string FoundationDate => foundationDate;
        [SerializeField] private string foundationDate = string.Empty;
        public string Name => name;
        [SerializeField] private string name = string.Empty;
        public bool IsCreated => isCreated;
        [SerializeField] private bool isCreated = false;

        public WarehouseData WarehouseData => warehouseData;
        [SerializeField] private WarehouseData warehouseData = new(-1);
        public OfficeData OfficeData => officeData;
        [SerializeField] private OfficeData officeData = new(-1);
        #endregion fields & properties

        #region methods
        public bool TryCreateCompany(string companyName)
        {
            if (IsCreated) return false;
            this.name = companyName;
            this.foundationDate = DateTime.Now.ToString("d");
            isCreated = true;
            return true;
        }
        #endregion methods
    }
}