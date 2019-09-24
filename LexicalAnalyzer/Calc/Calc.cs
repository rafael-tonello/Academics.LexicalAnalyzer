using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnalisadorLexico.Calc
{

    //lista de todos os tokens do analizador lexico
    

    public class TokenInfo
    {
        public string token;
        public string data;
        public int index;
        public int size;
    }

    //esta classe é a controladora. É responsável por receber uma empressão matemática e, atravez de subclasses do "Calc", 
    //realizar a validação
    class Calc
    {
        //lista de tokens e suas expressões regulares (!!!!!! usar get)
        public Dictionary<string, string> _tokens;

        //a tabela preditiva (!!!!!! usar get)
        public Dictionary<string, Dictionary<string, string[]>> _tabelaPreditiva;
        

        //ponteiros para os analisadores
        Lexico analisadorLexico;
        Sintatico analisadorSintatico;

        #region public

        public Calc()
        {
            analisadorLexico = new Lexico(this);
            analisadorSintatico = new Sintatico(this);
        }
        public void setTokens(Dictionary<string, string> tokens)
        {
            _tokens = tokens;
        }

        public void setTabelaPreditiva(Dictionary<string, Dictionary<string, string[]>> tabela)
        {
            _tabelaPreditiva = tabela;

            /*
            //linha para a expressão S->ID=EXP + EXP
            Dictionary<Tokens, Tokens[]> linha = new Dictionary<Tokens, Tokens[]>();
            linha.Add(Tokens.T_ID, new Tokens[] { Tokens.NT_EXPRESSAO, Tokens.T_OP_ADD, Tokens.NT_EXPRESSAO });
            tabela.Add(Tokens.NT_S, linha);
            +-----------------------------------------------------------+
            |     |         NT_EXPRESSAO          |          $          |
            +-----+-------------------------------+---------------------+
            |  S  |  NT_EXPRESSAO + NT_EXPRESSAO  |                     |
            +-----+-------------------------------+---------------------+
            */
        }


        public Lexico __getLexico()
        {
            return analisadorLexico;
        }

        #endregion


        //esta função é o ponto de entrada da validação de uma exmpressão
        public string validar(string func)
        {
            //pega os tokens (analise lexica)

            //faz a analise sintática

            return "";
        }


        //métodos úteis
        public bool getTokens(string expression, out List<TokenInfo> result, out MatchCollection invalidTokens)
        {
            return analisadorLexico.getTokens(expression, out result, out invalidTokens);
        }

        public bool analiseSintatica(Stack<string> pilha, Stack<string> expressao, out string erro, out int tokenErrorIndex)
        {
            return analisadorSintatico.validar(pilha, expressao, out erro, out tokenErrorIndex);
        }


    }
}

