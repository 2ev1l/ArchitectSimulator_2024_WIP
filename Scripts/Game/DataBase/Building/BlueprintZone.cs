namespace Game.DataBase
{
    public class BlueprintZone : PolygonBlueprintGraphic
    {
        #region fields & properties
        public bool IsSerializable
        {
            get => isSerializable;
            set => isSerializable = value;
        }
        private bool isSerializable = false;

        public bool IsTestCreated
        {
            get => isTestCreated;
            set => isTestCreated = value;
        }
        private bool isTestCreated = false;
        #endregion fields & properties

        #region methods
        protected override void OnCollisionUpdated() { }
        protected override void UpdatePlacement() { } //placement is constant
        protected override void UpdateTextureCoordinates() { } //don't required
        #endregion methods
    }
}