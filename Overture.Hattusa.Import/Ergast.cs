using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Overture.Hattusa.Data;

namespace Overture.Hattusa.Import
{

    /// <summary>
    /// Contains methods for importing data from http://ergast.com/.
    /// </summary>
    public class Ergast
    {

        #region Constants

        private const string URLFORMAT = "http://ergast.com/api/f1/{0}.{1}?limit={2}&offset={3}";
        private const string CONTENTFORMAT = "xml";

        #endregion

        #region Methods

        /// <summary>
        /// Extracts circuit data.
        /// </summary>
        public void ExtractCircuits()
        {
            Dictionary<string , List<Track>> tracksByCountry;
            Dictionary<string , Country> countries;
            int pageNumber;
            Country country;

            tracksByCountry = new Dictionary<string , List<Track>>();
            countries = GetCountriesFromDB().ToDictionary( c => c.Name , c => c );

            pageNumber = 0;
            while ( PopulateTracksFromPage( tracksByCountry , pageNumber ) )
            {
                pageNumber++;
            }

            foreach ( string countryName in tracksByCountry.Keys )
            {
                if ( countries.ContainsKey( countryName ) )
                {
                    country = countries[ countryName ];
                }
                else
                {
                    country = new Country() { Name = countryName };
                    AddCountryToDB( country );
                    countries.Add( countryName , country );
                }

                AddTracksToDB( country , tracksByCountry[ countryName ] );
            }
        }

        /// <summary>
        /// Populate dictionary of tracks from the source web page.
        /// </summary>
        /// <param name="tracksByCountry">Dictionary of tracks grouped by country to populate.</param>
        /// <param name="pageNumber">Number of source page to read.</param>
        /// <returns>True if there are more pages to read. False if the last page has been read.</returns>
        private static bool PopulateTracksFromPage( Dictionary<string , List<Track>> tracksByCountry , int pageNumber )
        {
            const string XMLNAMESPACE = "http://ergast.com/mrd/1.4";
            const int ROWSPERPAGE = 50;

            XPathDocument document;
            XPathNavigator navigator;
            XPathNodeIterator iterator;
            XmlNamespaceManager namespaceResolver;
            XPathNavigator location;
            Track track;
            string countryName;
            bool isLastPage;

            document = new XPathDocument( string.Format( URLFORMAT , "circuits" , CONTENTFORMAT , ROWSPERPAGE , pageNumber * ROWSPERPAGE ) );

            navigator = document.CreateNavigator();
            namespaceResolver = new XmlNamespaceManager( navigator.NameTable );
            namespaceResolver.AddNamespace( "ns" , XMLNAMESPACE );

            iterator = navigator.Select( "ns:MRData/ns:CircuitTable/ns:Circuit" , namespaceResolver );
            isLastPage = iterator.Count < ROWSPERPAGE;

            foreach ( XPathNavigator node in iterator )
            {
                track = new Track();
                track.Name = node.SelectSingleNode( "ns:CircuitName" , namespaceResolver )?.Value;
                location = node.SelectSingleNode( "ns:Location" , namespaceResolver );
                if ( location != null )
                {
                    track.Location = DbGeography.PointFromText( string.Format( "Point({0} {1})" , location.GetAttribute( "long" , "" ) , location.GetAttribute( "lat" , "" ) ) , 4326 );
                    track.City = location.SelectSingleNode( "ns:Locality" , namespaceResolver )?.Value;
                    countryName = location.SelectSingleNode( "ns:Country" , namespaceResolver )?.Value;
                    track.Country = new Country() { Name = countryName };

                    if ( !tracksByCountry.ContainsKey( countryName ) )
                    {
                        tracksByCountry.Add( countryName , new List<Track>() );
                    }
                    tracksByCountry[ countryName ].Add( track );
                }
            }

            return !isLastPage;
        }

        /// <summary>
        /// Gets all existing countries from the database.
        /// </summary>
        /// <returns>Array of countries.</returns>
        private static Country[] GetCountriesFromDB()
        {
            using ( F1Entities db = new F1Entities() )
            {
                return db.Countries.ToArray();
            }
        }

        /// <summary>
        /// Gets all existing tracks from the database.
        /// </summary>
        /// <returns>Array of tracks.</returns>
        private static Country[] GetTracksFromDB()
        {
            using ( F1Entities db = new F1Entities() )
            {
                return db.Countries.ToArray();
            }
        }

        /// <summary>
        /// Adds a new country to the database.
        /// </summary>
        /// <param name="country">Country to add.</param>
        private void AddCountryToDB( Country country )
        {
            if ( country.CountryId == 0 )
            {
                using ( F1Entities db = new F1Entities() )
                {
                    country.Tracks = null;
                    db.Countries.Add( country );
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Adds new tracks to the database.
        /// </summary>
        /// <param name="country">Country to which the tracks belong.</param>
        /// <param name="tracks">Tracks to add.</param>
        private void AddTracksToDB( Country country , IEnumerable<Track> tracks )
        {
            if ( country.CountryId != 0 && tracks != null )
            {
                using ( F1Entities db = new F1Entities() )
                {
                    foreach ( Track track in tracks )
                    {
                        track.CountryId = country.CountryId;
                        track.Country = null;
                        db.Tracks.Add( track );
                    }
                    db.SaveChanges();
                }
            }
        }

        //2.Ergast extraction code.
        //3.Azure Data Factory.

        #endregion

    }
}
