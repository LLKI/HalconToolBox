using System.CodeDom;
using System.Collections.ObjectModel;


namespace MachineVision.Core.TemplateMatch
{
    /// <summary>
    /// 定位服务处理结果
    /// </summary>
    public class MatchResult
    {
        /// <summary>
        /// 定位结果
        /// </summary>
        public bool IsSuccess =>Results.Count > 0;
        /// <summary>
        /// 耗时
        /// </summary>
        public double TimeSpan { get; set; }
        /// <summary>
        /// 目标结果
        /// </summary>
        public ObservableCollection<TemplateMatchResult> Results { get; set; }
    }
}
