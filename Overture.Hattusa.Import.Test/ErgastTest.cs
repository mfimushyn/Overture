using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Overture.Hattusa.Import.Test
{
    [TestClass]
    public class ErgastTest
    {
        public object Ergast { get; private set; }

        [TestMethod]
        public void ExtractCircuits_MustSucceed()
        {
            Ergast ergast;

            ergast = new Ergast();

            ergast.ExtractCircuits();
        }
    }
}
