using Prism.Mvvm;
using System.CodeDom;
using System.Collections.ObjectModel;


namespace MachineVision.Core.TemplateMatch
{
    /// <summary>
    /// 定位服务处理结果
    /// </summary>
    public class MatchResult:BindableBase
    {
        public MatchResult()
        {
            Setting = new MatchResultSetting();
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 耗时
        /// </summary>
        public double TimeSpan { get; set; }

        /// <summary>
        /// 匹配结果显示设置
        /// </summary>
        public MatchResultSetting Setting { get; set; }
        /// <summary>
        /// 目标结果
        /// </summary>
        public ObservableCollection<TemplateMatchResult> Results { get; set; }
    }

    /// <summary>
    /// 匹配结果设置
    /// </summary>
    public class MatchResultSetting : BindableBase
    {
        private bool isShowCenter = true;

        public bool IsShowCenter
        {
            get { return isShowCenter; }
            set { isShowCenter = value; RaisePropertyChanged(); }
        }

        private bool isShowDisplayText = true;

        public bool IsShowDisplayText
        {
            get { return isShowDisplayText; }
            set { isShowDisplayText = value; RaisePropertyChanged(); }
        }

        private bool isShowMatchRange = true;

        public bool IsShowMatchRange
        {
            get { return isShowMatchRange; }
            set { isShowMatchRange = value; RaisePropertyChanged(); }
        }

    }
}
