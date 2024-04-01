using System;
using Universal.Events;
using Game.DataBase;
using Game.UI.Text;

namespace Game.Events
{
    [System.Serializable]
    public class InfoRequest : ExecutableRequest
    {
        #region fields & properties
        public Action OnRejected;
        public string HeaderInfo;
        public string MainInfo;
        #endregion fields & properties

        #region methods
        /// <summary>
        /// 10X - Can't find player position. 0 => Location; 1 => Office; <br></br>
        /// 20X - Can't get data. 0 => Blueprint; <br></br>
        /// 30X - Infinity loop. 0 => Rooms/Find All; 1 => Rooms/Find Any; <br></br>
        /// 40X - Data error. 0 => Construction; <br></br>
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static InfoRequest GetErrorRequest(int errorCode)
        {
            InfoRequest request = new(null, new LanguageInfo(-1, TextType.None).Text, $"{new LanguageInfo(66, TextType.Menu).Text}\n[{errorCode}]");
            return request;
        }
        public override void Close()
        {
            OnRejected?.Invoke();
        }
        public InfoRequest() { }
        public InfoRequest(Action onRejected, LanguageInfo headerInfo, LanguageInfo mainInfo)
        {
            this.OnRejected = onRejected;
            this.HeaderInfo = headerInfo.Text;
            this.MainInfo = mainInfo.Text;
        }
        public InfoRequest(Action onRejected, string headerInfo, string mainInfo)
        {
            this.OnRejected = onRejected;
            this.HeaderInfo = headerInfo;
            this.MainInfo = mainInfo;
        }
        #endregion methods
    }
}