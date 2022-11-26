/*Ramírez Tovar Carlos*/
using System;
using System.Collections.Generic;
//Requerimiento 1. Construir un metodo para escribir en el archivo Lenguaje.cs identando el codigo
//                 "{" incrementa un tabulador, "}" decrementa un tabulador V
//Requerimiento 2. Declarar un atributo "primeraProduccion" de tipo string y actualizarlo con la 
//                 primera produccion de la gramatica V 
//Requerimiento 3. La primera produccion es publica y el resto es privada V 
//Requerimiento 4. El constructor lexico parametrico debe validar que la extensión del archivo a compilar
                 //sea .gen y sino levantar una exception
//Requerimiento 5. Resolver la ambiguedad de st y snt

namespace Generador
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        private string tabulador, primeraProduccion;
        private List<string> stackSnt = new List<string>();
        public Lenguaje(string nombre) : base(nombre)
        {
            tabulador = "";
            primeraProduccion = "public ";
        }
        public Lenguaje()
        {
            tabulador = "";
            primeraProduccion = "public ";
        }
        public void Dispose()
        {
            cerrar();
        }
        private void NoTerminales()
        {
            int pos = posicion - getContenido().Length; 
            while(archivo.Peek() > -1)
            {
                stackSnt.Add(getContenido());
                archivo.ReadLine();
                NextToken();
            } 
            archivo.DiscardBufferedData(); 
            archivo.BaseStream.Seek(pos, System.IO.SeekOrigin.Begin);
            NextToken(); 
        }
        private bool esSNT(string produccion)
        {
            foreach (var noTerminal in stackSnt)
            {
                if(noTerminal == produccion)
                {
                    return true;
                }
            }
            return false; 
        }
        private void identado(char contenido)
        {
            if (contenido == '{')
            {
                tabulador += '\t';
            }
            else if (contenido == '}')
            {
                tabulador = tabulador.Remove(tabulador.Length - 1);
            }
        }
        private void Programa(string produccionPrincipal)
        {
            programa.WriteLine("using System;");
            programa.WriteLine("using System.IO;");
            programa.WriteLine("using System.Collections.Generic;");
            programa.WriteLine();
            programa.WriteLine("namespace Generico");
            programa.WriteLine("{");
            identado('{');
            programa.WriteLine(tabulador + "public class Program");
            programa.WriteLine(tabulador + "{");
            identado('{');
            programa.WriteLine(tabulador + "static void Main(string[] args)");
            programa.WriteLine(tabulador + "{");
            identado('{');
            programa.WriteLine(tabulador + "try");
            programa.WriteLine(tabulador + "{");
            identado('{');
            programa.WriteLine(tabulador + "using (Lenguaje a = new Lenguaje())");
            programa.WriteLine(tabulador + "{");
            identado('{');
            programa.WriteLine(tabulador + "a." + produccionPrincipal + "();");
            identado('}');
            programa.WriteLine(tabulador + "}");
            identado('}');
            programa.WriteLine(tabulador + "}");
            programa.WriteLine(tabulador + "catch (Exception e)");
            programa.WriteLine(tabulador + "{");
            identado('{');
            programa.WriteLine(tabulador + "Console.WriteLine(e.Message);");
            identado('}');
            programa.WriteLine(tabulador + "}");
            identado('}');
            programa.WriteLine(tabulador + "}");
            identado('}');
            programa.WriteLine(tabulador + "}");
            identado('}');
            programa.WriteLine("}");
        }
        public void gramatica()
        {
            cabecera();
            Programa(getContenido());
            cabeceraLenguaje();
            NoTerminales(); 
            listaProducciones();
            identado('}');
            lenguaje.WriteLine(tabulador + "}");
            lenguaje.WriteLine("}");
        }
        private void cabecera()
        {
            match("Gramatica");
            match(":");
            match(Tipos.SNT);
            match(Tipos.FinProduccion);
        }
        private void cabeceraLenguaje()
        {
            lenguaje.WriteLine("using System;");
            lenguaje.WriteLine("using System.Collections.Generic;");
            lenguaje.WriteLine("namespace Generico");
            lenguaje.WriteLine("{");
            identado('{');
            lenguaje.WriteLine(tabulador + "public class Lenguaje : Sintaxis, IDisposable");
            lenguaje.WriteLine(tabulador + "{");
            identado('{');
            lenguaje.WriteLine(tabulador + "string nombreProyecto;");
            lenguaje.WriteLine(tabulador + "public Lenguaje(string nombre) : base(nombre)");
            lenguaje.WriteLine(tabulador + "{");
            identado('{'); 
            identado('}');
            lenguaje.WriteLine(tabulador + "}");
            lenguaje.WriteLine(tabulador + "public Lenguaje()");
            lenguaje.WriteLine(tabulador + "{");
            identado('{');
            identado('}');
            lenguaje.WriteLine(tabulador + "}");
            lenguaje.WriteLine(tabulador + "public void Dispose()");
            lenguaje.WriteLine(tabulador + "{");
            identado('{');
            lenguaje.WriteLine(tabulador + "cerrar();");
            identado('}');
            lenguaje.WriteLine(tabulador + "}");
        }
        private void listaProducciones()
        {
            lenguaje.WriteLine(tabulador + primeraProduccion + "void "+getContenido()+"()");
            primeraProduccion = "private ";
            lenguaje.WriteLine(tabulador + "{");
            identado('{');
            match(Tipos.SNT);
            match(Tipos.Produce);
            simbolos(); 
            match(Tipos.FinProduccion);
            identado('}');
            lenguaje.WriteLine(tabulador + "}");
            if (!FinArchivo())
            {
                listaProducciones();
            }
        }
        private void simbolos()
        {
            if(!esSNT(getContenido()))
            {
                setClasificacion(Tipos.ST);
            }
            if(esTipo(getContenido()))
            {
                lenguaje.WriteLine(tabulador + "match(Tipos." + getContenido() +");");
                match(Tipos.ST); //verificar
            }
            else if(getClasificacion() == Tipos.ST)
            {
                lenguaje.WriteLine(tabulador + "match(\"" + getContenido() +"\");");
                match(Tipos.ST);
            }
            else if(getClasificacion() == Tipos.SNT)
            {
                lenguaje.WriteLine(tabulador + getContenido() +"();");
                match(Tipos.SNT);
            }
            if(getClasificacion() != Tipos.FinProduccion)
            {
                simbolos(); 
            }
        }

        private bool esTipo(String clasificacion){
            switch (clasificacion)
            {
                case "Identificador":
                case "Numero": 
                case "Caracter": 
                case "Asignacion":
                case "Inicializacion":
                case "OperadorLogico": 
                case "OperadorRelacional": 
                case "OperadorTernario": 
                case "OperadorTermino": 
                case "OperadorFactor": 
                case "IncrementoTermino": 
                case "IncrementoFactor": 
                case "FinSentencia": 
                case "Cadena": 
                case "TipoDato":
                case "Zona": 
                case "Condicion":
                case "Ciclo":
                return true;
            }
            return false; 
        }
    }
}