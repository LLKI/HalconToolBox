using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.TemplateMatch
{
    /// <summary>
    /// 模板匹配接口
    /// </summary>
    public interface ITemplateMatchService
    {
        /// <summary>
        /// 模板匹配信息描述
        /// </summary>
        MethodInfo info { get; set; }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="hObject">生成模板的指定区域图像</param>
        /// <returns></returns>
        Task CreateTemplate(HObject Image,HObject hObject);

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="Image"></param>
        MatchResult Run(HObject Image);
    }
}
