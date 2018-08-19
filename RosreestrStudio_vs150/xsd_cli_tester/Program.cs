using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using netFteo.XML;

namespace xsd_cli_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("@Fixosoft 12.11.2017  home153@mail.ru");
            Console.WriteLine("Пример использования класса netFteo.XML.XSDEnumFile\r\n");
            //urn://x-artefacts-rosreestr-ru/commons/directories/regions/1.0.1
            netFteo.XML.XSDFile xsdenum = new netFteo.XML.XSDFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\Schema\\SchemaCommon\\dRegionsRF_v01.xsd");
            if (xsdenum.EnumerationPresent)
            {
                Console.WriteLine("\n\r Schema simple type: " + xsdenum.SimpleTypeNames.Count().ToString());
                foreach (string s in xsdenum.SimpleTypeNames)
                    Console.WriteLine(s);
                Console.WriteLine("\n\r Schema targetNamespace: \n\r " + xsdenum.targetNamespace);
                List<string> enumAnnot = xsdenum.Item2Annotation("26", xsdenum.SimpleTypeNamesSafeFirst);
                Console.WriteLine("\n\rEnumerated for 26 : " + enumAnnot.Count().ToString());
                foreach (string s in enumAnnot)
                    Console.WriteLine(s);
                List<string> enumAnnotFull = xsdenum.Item2Annotation("", xsdenum.SimpleTypeNamesSafeFirst);
                Console.WriteLine("\n\rEnumerated for All : " + enumAnnotFull.Count().ToString());
                foreach (string s in enumAnnotFull)
                    Console.WriteLine(s);
            }
            //Wait Exit with escape press:
            Console.WriteLine("\n\rPress ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    // Do something
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
