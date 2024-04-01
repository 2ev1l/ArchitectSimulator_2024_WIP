namespace Game.Serialization.Settings.Input
{
    public enum KeyCodeDescription
    {
        None,
        MoveForward,
        MoveBackward,
        MoveRight,
        MoveLeft,
        Interact,
        OpenSettings,
        Run,
        Info,
        HideUI,
        DesignMoveUp,
        DesignMoveDown,
        DesignMoveRight,
        DesignMoveLeft,
        DesignRotate,
        DesignDeselect,
        DesignRemove,
        DesignDuplicate,
        DesignFocus,
    }
    public static class KeyCodeDescriptionExtensions
    {
        /// <summary>
        /// Ref 'menu' text
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static int GetLanguageTextId(this KeyCodeDescription description) => description switch
        {
            KeyCodeDescription.None => -1,
            KeyCodeDescription.MoveForward => 27,
            KeyCodeDescription.MoveBackward => 28,
            KeyCodeDescription.MoveRight => 29,
            KeyCodeDescription.MoveLeft => 30,
            KeyCodeDescription.Interact => 31,
            KeyCodeDescription.OpenSettings => 2,
            KeyCodeDescription.Run => 54,
            KeyCodeDescription.Info => 63,
            KeyCodeDescription.HideUI => 64,
            KeyCodeDescription.DesignMoveUp => 69,
            KeyCodeDescription.DesignMoveDown => 70,
            KeyCodeDescription.DesignMoveRight => 71,
            KeyCodeDescription.DesignMoveLeft => 72,
            KeyCodeDescription.DesignRotate => 73,
            KeyCodeDescription.DesignDeselect => 74,
            KeyCodeDescription.DesignRemove => 75,
            KeyCodeDescription.DesignDuplicate => 76,
            KeyCodeDescription.DesignFocus => 77,
            _ => -1
        };
    }
}