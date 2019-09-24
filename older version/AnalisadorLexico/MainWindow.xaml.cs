using AnalisadorLexico.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnalisadorLexico
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush originalCOlor = null;
        public MainWindow()
        {
            InitializeComponent();
            lbExpMensagem.Visibility = Visibility.Hidden;
            originalCOlor = lbExpTitulo.Foreground;
            tbTokens.Text = "";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            hideExpressionError();
            List<Calc.TokenInfo> result;
            MatchCollection invalidTokens;

            Calc.Calc processador = new Calc.Calc();

            //seta os tokens
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            //expressões regulares para encontrar os tokens
            tokens.Add("[a-z]", "[a-zA-Z_][a-zA-Z0-9]*");
            tokens.Add("[0-9]", @"[-]?[\d]_*\.?[\d]*E?");
            tokens.Add("(", @"[\(]");
            tokens.Add(")", @"[\)]");
            tokens.Add("=", "=");
            tokens.Add("+", @"\+");
            tokens.Add("-", @"\-");
            tokens.Add("*", @"\*");
            tokens.Add("/", @"\/");
            tokens.Add("^", @"\^");

            processador.setTokens(tokens);

            Dictionary<string, string[]> tempLinha;
            #region  seta a tabela preditiva
                Dictionary<string, Dictionary<string, string[]>> tabelaPreditiva = new Dictionary<string, Dictionary<string, string[]>>();
                
                #region linha S, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna [a-z]
                    tempLinha.Add("[a-z]", new string[] { "[a-z]", "=", "EXP" });
                    //coluna $
                    tempLinha.Add("$", new string[] { });


                    //adiciona os erros
                    tempLinha.Add("=", new string[] { "!!!ERROR:Identificador esperado."});
                    tempLinha.Add("+", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add("-", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add("*", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add("/", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add("^", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add("(", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add(")", new string[] { "!!!ERROR:Identificador esperado." });
                    tempLinha.Add("[0-9]", new string[] { "!!!ERROR:Identificador esperado." });

                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("S", tempLinha);
                #endregion

                #region linha EXP, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna [a-z]
                    tempLinha.Add("[a-z]", new string[] { "TERM", "EXPL"});
                    //coluna (
                    tempLinha.Add("(", new string[] { "TERM", "EXPL" });
                    //coluna [0-9]
                    tempLinha.Add("[0-9]", new string[] { "TERM", "EXPL" });

                    //adiciona os erros
                    tempLinha.Add("=", new string[] { "!!!ERROR:Identificador, número ou '(' esperado."});
                    tempLinha.Add("+", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("-", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("*", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("/", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("^", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add(")", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("$", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    

                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("EXP", tempLinha);
                #endregion

                #region linha EXPL, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna +
                    tempLinha.Add("+", new string[] { "+", "TERM", "EXPL"});
                    //coluna -
                    tempLinha.Add("-", new string[] { "-", "TERM", "EXPL" });
                    //coluna )
                    tempLinha.Add(")", new string[] { });
                    //coluna $
                    tempLinha.Add("$", new string[] { });

                    //adiciona os erros
                    tempLinha.Add("[a-z]", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão" });
                    tempLinha.Add("=", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão"});
                    tempLinha.Add("*", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão" });
                    tempLinha.Add("/", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão" });
                    tempLinha.Add("^", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão" });
                    tempLinha.Add("(", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão" });
                    tempLinha.Add("[0-9]", new string[] { "!!!ERROR:Esperado adição, subtração, '(' ou fim da expressão" });
                    

                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("EXPL", tempLinha);
                #endregion

                #region linha TERM, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna [a-z]
                    tempLinha.Add("[a-z]", new string[] { "FATOR", "TERML"});
                    //coluna (
                    tempLinha.Add("(", new string[] { "FATOR", "TERML"});
                    //coluna )
                    tempLinha.Add("[0-9]", new string[] { "FATOR", "TERML"});
            
                    //adiciona os erros
                    tempLinha.Add("=", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("+", new string[] { "!!!ERROR:Esperado identificador, numero ou '('" });
                    tempLinha.Add("-", new string[] { "!!!ERROR:Esperado identificador, numero ou '('" });
                    tempLinha.Add("*", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("/", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("^", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add(")", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("$", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});


                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("TERM", tempLinha);
                #endregion

                #region linha TERML, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna +
                    tempLinha.Add("+", new string[] { });
                    //coluna -
                    tempLinha.Add("-", new string[] { });
                    //coluna *
                    tempLinha.Add("*", new string[] { "*", "FATOR", "TERML"});
                    //coluna /
                    tempLinha.Add("/", new string[] { "/", "FATOR", "TERML" });
                    //coluna )
                    tempLinha.Add(")", new string[] { });
                    //coluna $
                    tempLinha.Add("$", new string[] { });
            
                    //adiciona os erros
                    tempLinha.Add("[a-z]", new string[] { "!!!ERROR:Esperado fim operação, ')' ou fim da expressão" });
                    tempLinha.Add("=", new string[] { "!!!ERROR:Esperado fim operação, ')' ou fim da expressão" });
                    tempLinha.Add("^", new string[] { "!!!ERROR:Esperado fim operação, ')' ou fim da expressão" });
                    tempLinha.Add("[0-9]", new string[] { "!!!ERROR:Esperado fim operação, ')' ou fim da expressão" });

                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("TERML", tempLinha);
                #endregion

                #region linha FATOR, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna [a-z]
                    tempLinha.Add("[a-z]", new string[] { "NUMERO", "FATORL" });
                    //coluna (
                    tempLinha.Add("(", new string[] { "NUMERO", "FATORL" });
                    //coluna [0-9]
                    tempLinha.Add("[0-9]", new string[] { "NUMERO", "FATORL" });


                    //adiciona os erros
                    
                    tempLinha.Add("=", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("+", new string[] { "!!!ERROR:Esperado identificador, numero ou '('" });
                    tempLinha.Add("-", new string[] { "!!!ERROR:Esperado identificador, numero ou '('" });
                    tempLinha.Add("*", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("/", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("^", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add(")", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});
                    tempLinha.Add("$", new string[] { "!!!ERROR:Esperado identificador, numero ou '('"});

                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("FATOR", tempLinha);
                #endregion

                 #region linha FATORL, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna +
                    tempLinha.Add("+", new string[] { });
                    //coluna -
                    tempLinha.Add("-", new string[] { });
                    //coluna *
                    tempLinha.Add("*", new string[] { });
                    //coluna /
                    tempLinha.Add("/", new string[] { });
                    //coluna ^
                    tempLinha.Add("^", new string[] { "^", "NUMERO", "FATORL" });
                    //coluna )
                    tempLinha.Add(")", new string[] { });
                    //coluna $
                    tempLinha.Add("$", new string[] { });

                    //adiciona os erros
                    tempLinha.Add("[a-z]", new string[] { "!!!ERROR:Esperado fim operação, ')', '^' ou fim da expressão" });
                    tempLinha.Add("=", new string[] { "!!!ERROR:Esperado fim operação, ')', '^' ou fim da expressão" });
                    tempLinha.Add("(", new string[] { "!!!ERROR:Esperado fim operação, ')', '^' ou fim da expressão" });
                    tempLinha.Add("[0-9]", new string[] { "!!!ERROR:Esperado fim operação, ')' ou fim da expressão" });
                    
                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("FATORL", tempLinha);
                #endregion

                #region linha NUMERO, 
                    tempLinha = new Dictionary<string, string[]>();
                    //coluna [a-z]
                    tempLinha.Add("[a-z]", new string[] { "[a-z]" });
                    //coluna (
                    tempLinha.Add("(", new string[] { "(", "EXP", ")" });
                    //coluna [0-9]
                    tempLinha.Add("[0-9]", new string[] { "[0-9]"});
                    
                    //adiciona os erros
                    tempLinha.Add("=", new string[] { "!!!ERROR:Identificador, número ou '(' esperado."});
                    tempLinha.Add("+", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("-", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("*", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("/", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("^", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add(")", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });
                    tempLinha.Add("$", new string[] { "!!!ERROR:Identificador, número ou '(' esperado." });

                    //depois de completa, adiciona a linha à tabel
                    tabelaPreditiva.Add("NUMERO", tempLinha);
                #endregion

                processador.setTabelaPreditiva(tabelaPreditiva);
            #endregion



            //carrega os tokens, para mostrá-los na tela
            if (processador.getTokens(tbFormula.Text, out result, out invalidTokens))
            {
                tbTokens.Clear();
                for (int cont = 0; cont < result.Count; cont++)
                {
                    tbTokens.Text += result[cont].token.ToString() + ": " + result[cont].data + "\r\n";

                }
            }
            else
            {
                showExpressionError("Há algo erro na expressão.");

                
            }

            //realisa um processamento sintático, para tests
            //converte a lista de TokenInfo para uma pilha de string (isso será feito pelo Calc, posteriormente)
            Stack<string> expressao = new Stack<string>();
            expressao.Push("$");
            int tokenErrorIndex;
            for (int cont = result.Count-1; cont >= 0; cont--)
                expressao.Push(result[cont].token);


            //cria uma pilha, para validar a expressão digitada
            Stack<string> pilha = new Stack<string>();
            pilha.Push("$");
            pilha.Push("S");
            string erro;
            if (processador.analiseSintatica(pilha, expressao, out erro, out tokenErrorIndex))
                MessageBox.Show("Aceitou a expressão");
            else
            {
                MessageBox.Show("Erro na expressão:\n\t" + erro);
                int indiceErro = result.Count - tokenErrorIndex + 1;
                if (indiceErro > result.Count - 1)
                    indiceErro = result.Count - 1;
                MessageBox.Show(result[indiceErro].data);
            }
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            button_Click(null, null);
        }

        private void showExpressionError(string msg)
        {
            Color cor = Color.FromRgb(255, 120, 120);
            lbExpMensagem.Foreground = new SolidColorBrush(cor);
            lbExpTitulo.Foreground = new SolidColorBrush(cor);
            cvExpBase.Background = new SolidColorBrush(cor);
            lbExpMensagem.Content = msg;
            lbExpMensagem.Visibility = Visibility.Visible;

        }

        private void hideExpressionError()
        {
            lbExpMensagem.Foreground = originalCOlor;
            lbExpTitulo.Foreground = originalCOlor;
            cvExpBase.Background = originalCOlor;
            lbExpMensagem.Visibility = Visibility.Hidden;

        }



        public void setTimeout(Action acao, int timeout, System.Windows.Threading.Dispatcher invoker)
        {
            Thread temp = new Thread(delegate ()
                {
                    Thread.Sleep(timeout);
                    invoker.Invoke(acao);
                }
            );
        }
    }
}
