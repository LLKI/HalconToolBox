using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.TemplateMatch
{
    /// <summary>
    /// 算子参数
    /// </summary>
    public class MethodInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<MethodParmeter> Parmeters { get; set; }

        /// <summary>
        /// 关联算子
        /// </summary>
        public List<string> Predecessors { get; set; }
    }

    public class MethodParmeter
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Parameters { get; set; }
    }
}
