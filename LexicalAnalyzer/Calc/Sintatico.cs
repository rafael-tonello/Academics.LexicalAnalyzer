using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnalisadorLexico.Calc
{
    class Sintatico
    {
        Calc ctrl;
        public Sintatico(Calc pCtrl)
        {
            ctrl = pCtrl;
        }


        

        public bool validar(Stack<string> pilha, Stack<string> expressao, out string erro, out int tokenErrorIndex)
        {
            //verifica se já a pilha e a expressão possuem o $ no inicio 
            string[] substituir; 
            while ((pilha.Count > 0) && (expressao.Count > 0))
            {
                if (pilha.Peek() == expressao.Peek())
                {
                    pilha.Pop();
                    expressao.Pop();
                }
                else
                {
                    try
                    {
                        //localiza a substituição na tabela preditiva
                        substituir = ctrl._tabelaPreditiva[pilha.Peek()][expressao.Peek()];
                    }
                    //por se tratar de um dicionário, quando não houver a correspondência, uma excessão será lançada
                    catch { substituir = null; }

                    //verifica se encontrou algum erro


                    if ((substituir == null) || (substituir.Length > 0 && substituir[0].IndexOf("!!!ERROR:") > -1))
                    {
                        //==============================================================
                        //!!!!!!!!!!! fazer o tratamento de erros aqui!!!!!!!!!!!!!!!!!!
                        //==============================================================
                        tokenErrorIndex = expressao.Count;
                        if (substituir == null)
                            erro = "Não foi possível identificar o erro";
                        else
                            erro = substituir[0].Substring(substituir[0].IndexOf(':') + 1);
                            return false;
                    }
                    else
                    {
                        //substiu o último elemento da pilha com os elementos encontrados na tabela
                        pilha.Pop();
                        for (int cont = substituir.Length - 1; cont >= 0; cont--)
                            pilha.Push(substituir[cont]);

                        //quando a tabela tiver ums substituição vazia (sem erros) ela vai possui um vetor vazio, logo
                        //a pilha terá seu últio elemento removido e nenhum reposto
                    }
                    

                }
            }

            erro = "";
            tokenErrorIndex = -1;
            return (pilha.Count == 0) && (expressao.Count == 0);
        }
    }
}
