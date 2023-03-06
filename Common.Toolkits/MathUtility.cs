namespace Common.Toolkits
{
    public static class MathUtility
    {
        public static int MaximalDivisor(int data1, int data2)
        {
            #region 辗转相除法
            int data3 = 0;
            while (data1 % data2 != 0)
            {
                data3 = data1 % data2;
                data1 = data2;
                data2 = data3;
            }
            return data2;
            #endregion

            #region
            //int max = 0;
            //int min = 0;
            //int i = (d1 < d2 ? d1 : d2);
            //max = (d1 > d2 ? d1 : d2); ;
            //min = i;
            //while (i != 0)
            //{
            //    if ((max % i == 0) && (min % i == 0))
            //    {
            //        break;
            //    }
            //    i--;
            //}
            //return i;
            #endregion
            
            // 两数之积/最大公约数=最小公倍数
        }
    }
}
