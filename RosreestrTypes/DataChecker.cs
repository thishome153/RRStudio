using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RRTypes.MapPlanV01;
using RRTypes.kpt09;
using RRTypes.kvzu;

#region Типы к Схемам Росреестра

namespace RRTypes
{
    /// <summary>
    /// Class controller for output files for Rosreestr
    /// Checks files integrity, data constrainst and logical relations
    /// </summary>
    public class DataChecker
    {
        private object fDocument;
        public DataChecker(object document)
        {
            this.fDocument = document;
        }

        public string Type
        {
            get
            {
                return this.fDocument.GetType().Name;
            }
        }

        public void Integrity()
        {

        }

        public void TestForeignFiles()
        {

        }

        public bool TestGUID(Guid Guid)
        {
            return false;
        }

        public bool FilePresent(string FilePath)
        {
            return false;
        }
    }
     
}
#endregion
