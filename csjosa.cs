using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Myevan
{
    public class Korean
    {
        public static bool HasJong(char inChar)
        {
            if (inChar >= 0xAC00 && inChar <= 0xD7A3) // 가 ~ 힣
            {
                int localCode = inChar - 0xAC00; // 가~ 이후 로컬 코드 
                int jongCode = localCode % 28;
                if (jongCode > 0)
                {
                    if (jongCode == 8) // ㄹ 종성 예외 처리
                        return false;
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        class JosaPair
        {
            public JosaPair(string josa1, string josa2)
            {
                this.josa1 = josa1;
                this.josa2 = josa2;
            }

            public string josa1
            { get; private set; }

            public string josa2
            { get; private set; }
        }

        public static string ReplaceJosa(string src)
        {
            var josaRegex = new Regex(@"\(이\)가|\(을\)를|\(은\)는|\(으\)로");
            var josaPaird = new Dictionary<string, JosaPair>();
            josaPaird.Add("(이)가", new JosaPair("이", "가"));
            josaPaird.Add("(을)를", new JosaPair("을", "를"));
            josaPaird.Add("(은)는", new JosaPair("은", "는"));
            josaPaird.Add("(으)로", new JosaPair("으로", "로"));

            var strBuilder = new StringBuilder(src.Length);
            var josaMatches = josaRegex.Matches(src);
            var lastHeadIndex = 0;
            foreach (Match josaMatch in josaMatches)
            {
                var josaPair = josaPaird[josaMatch.Value];

                strBuilder.Append(src, lastHeadIndex, josaMatch.Index - lastHeadIndex);
                if (josaMatch.Index > 0)
                {
                    var prevChar = src[josaMatch.Index - 1];
                    if (HasJong(prevChar))
                    {
                        strBuilder.Append(josaPair.josa1);
                    }
                    else
                    {
                        strBuilder.Append(josaPair.josa2);
                    }
                }
                else
                {
                    strBuilder.Append(josaPair.josa1);
                }

                lastHeadIndex = josaMatch.Index + josaMatch.Length;
            }
            strBuilder.Append(src, lastHeadIndex, src.Length - lastHeadIndex);
            return strBuilder.ToString();
        }
    }
}
