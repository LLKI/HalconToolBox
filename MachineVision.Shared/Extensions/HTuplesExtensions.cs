using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Shared.Extensions
{
    public static class HTuplesExtensions
    {
        public static HObject GenRectangle(this HTuple[] htuples)
        {
            HObject drawObj;
            HOperatorSet.GenEmptyObj(out drawObj);

            if (htuples[0].D!=0 && htuples[1].D!=0 && htuples[2].D!=0 && htuples[3].D !=0)
            {
                HOperatorSet.GenRectangle1(out drawObj, htuples[0], htuples[1], htuples[2], htuples[3]);
                return drawObj;
            }
            return null;
        }

        public static HObject GenCircle(this HTuple[] htuples)
        {
            HObject drawObj;
            HOperatorSet.GenEmptyObj(out drawObj);

            if (htuples[0].D != 0 && htuples[1].D != 0 && htuples[2].D != 0)
            {
                HOperatorSet.GenCircle(out drawObj, htuples[0], htuples[1], htuples[2]);
                return drawObj;
            }
            return null;
        }

        public static HObject GenEllipse(this HTuple[] htuples)
        {
            HObject drawObj;
            HOperatorSet.GenEmptyObj(out drawObj);

            if (htuples[0].D != 0 && htuples[1].D != 0 && htuples[2].D != 0 && htuples[3].D != 0 && htuples[4].D !=0)
            {
                HOperatorSet.GenEllipse(out drawObj, htuples[0], htuples[1], htuples[2], htuples[3], htuples[4]);
                return drawObj;
            }
            return null;
        }
    }
}
