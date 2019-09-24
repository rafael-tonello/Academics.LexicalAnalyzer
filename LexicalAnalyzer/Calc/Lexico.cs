using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnalisadorLexico.Calc
{
    class Lexico
    {
        Calc ctrl;

        public Lexico(Calc pCtrl)
        {
            ctrl = pCtrl;
        }
        #region analise léxica (mover para uma classe)

        //a função abaixo extrai os tokens de uma expressão matemática
        public bool getTokens(string expression, out List<TokenInfo> result, out MatchCollection invalidTokens)
        {
            MatchCollection temp;
            string originalExpression = expression;
            result = new List<TokenInfo>();
            for (int cont = 0; cont < ctrl._tokens.Count; cont++)
            {
                temp = System.Text.RegularExpressions.Regex.Matches(expression, ctrl._tokens[ctrl._tokens.ElementAt(cont).Key]);
                expression = replaceByChar(expression, temp);

                for (int cont2 = 0; cont2 < temp.Count; cont2++)
                    result.Add(new TokenInfo { token = ctrl._tokens.ElementAt(cont).Key, data = originalExpression.Substring(temp[cont2].Index, temp[cont2].Length), index = temp[cont2].Index, size = temp[cont2].Length });
            }


            if (expression.Trim() == "")
            {
                result.Sort(delegate (TokenInfo a, TokenInfo b) { return a.index.CompareTo(b.index); });
                invalidTokens = null;
                return true;
            }
            else
            {
                //identifica as posições dos caracteres inválidos (usa [^\s], que localiza tudo o que for diferente de espaço)
                //result = new Dictionary<Tokens, string>();
                invalidTokens = System.Text.RegularExpressions.Regex.Matches(expression, @"[^\s]");
                return false;
            }
            
        }

        private string replaceByChar(string expression, MatchCollection matches, char replaceBy = ' ')
        {
            StringBuilder temp = new StringBuilder(expression);
            for (int cont = 0; cont < matches.Count; cont++)
            {
                for (int cont2 = matches[cont].Index; cont2 < matches[cont].Index + matches[cont].Length; cont2++)
                {
                    temp[cont2] = replaceBy;
                }
            }

            return temp.ToString();
        }
        #endregion
    }
}
