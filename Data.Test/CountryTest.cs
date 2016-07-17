using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Overture.Hattusa.Data.Test
{
    [TestClass]
    public class CountryTest
    {
        [TestMethod]
        public void AddCountry_MustSucceed()
        {
            Country country;

            using ( F1Entities db = new F1Entities() )
            {
                db.Database.Log = s => Trace.Write( s );

                country = new Country()
                {
                    Name = "Italy" 
                };
                db.Countries.Add( country );
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void ReadCountries_MustSucceed()
        {
            using ( F1Entities db = new F1Entities() )
            {
                db.Database.Log = s => Trace.Write( s );

                Country[] countries = db.Countries.ToArray();
            }
        }

        [TestMethod]
        public void ReadCountryById_MustSucceed()
        {
            using ( F1Entities db = new F1Entities() )
            {
                db.Database.Log = s => Trace.Write( s );

                Country country = db.Countries.Find( 1 );
                Assert.AreNotEqual( country , null );
            }
        }
    }
}
