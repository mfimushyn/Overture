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
        private const int ROWSPERPAGE = 50;

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
            return PopulateFromPage(
                ROWSPERPAGE ,
                pageNumber ,
                "circuits" ,
                "ns:MRData/ns:CircuitTable/ns:Circuit" ,
                ( node , namespaceResolver ) =>
                {
                    Track track;
                    string countryName;
                    XPathNavigator location;

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
                } );
        }


        /// <summary>
        /// Populate dictionary of races from the source web page.
        /// </summary>
        /// <param name="racesByTrack">Dictionary of races grouped by track to populate.</param>
        /// <param name="pageNumber">Number of source page to read.</param>
        /// <returns>True if there are more pages to read. False if the last page has been read.</returns>
        private static bool PopulateRacesFromPage( Dictionary<string , List<Race>> racesByTrack , int pageNumber )
        {
            return PopulateFromPage(
                ROWSPERPAGE ,
                pageNumber ,
                "races" ,
                "ns:MRData/ns:RaceTable/ns:Race" ,
                ( node , namespaceResolver ) =>
                {
                    Race race;
                    GrandPrix gp;
                    string trackName;
                    XPathNavigator circuit;
                    XPathNavigator location;

                    gp = new GrandPrix();
                    gp.Name = node.SelectSingleNode( "ns:RaceName" , namespaceResolver )?.Value;

                    race = new Race();
                    race.GrandPrix = gp;
                    race.Date = DateTime.Parse( node.SelectSingleNode( "ns:Date" , namespaceResolver )?.Value );
                    race.Season = Convert.ToInt16( race.Date.Year );
                    circuit = node.SelectSingleNode( "ns:Circuit" , namespaceResolver );
                    if ( circuit != null )
                    {
                        trackName = circuit.SelectSingleNode( "ns:CircuitName" , namespaceResolver )?.Value;
                        race.Track = new Track() { Name = trackName };

                        location = circuit.SelectSingleNode( "ns:Location" , namespaceResolver );
                        if ( location != null )
                        {
                            gp.Country = new Country() { Name = location.SelectSingleNode( "ns:Country" , namespaceResolver )?.Value };
                        }

                        if ( !racesByTrack.ContainsKey( trackName ) )
                        {
                            racesByTrack.Add( trackName , new List<Race>() );
                        }
                        racesByTrack[ trackName ].Add( race );
                    }
                } );
        }

        /// <summary>
        /// Creates XPathNodeIterator for traversing source page XML elements.
        /// </summary>
        /// <param name="rowsPerPage">Number of records per page.</param>
        /// <param name="pageNumber">Number of requested page.</param>
        /// <param name="sourcePageName">Name of the source page.</param>
        /// <param name="elementXPath">XPath for selecting elements.</param>
        /// <returns>True if there are more records to process on the next page.</returns>
        private static bool PopulateFromPage( int rowsPerPage , int pageNumber , string sourcePageName , string elementXPath , Action<XPathNavigator , XmlNamespaceManager> recordHandler )
        {
            const string XMLNAMESPACE = "http://ergast.com/mrd/1.4";

            XPathDocument document;
            XPathNavigator navigator;
            XmlNamespaceManager namespaceResolver;
            XPathNodeIterator iterator;

            document = new XPathDocument( string.Format( URLFORMAT , sourcePageName , CONTENTFORMAT , rowsPerPage , pageNumber * rowsPerPage ) );

            navigator = document.CreateNavigator();
            namespaceResolver = new XmlNamespaceManager( navigator.NameTable );
            namespaceResolver.AddNamespace( "ns" , XMLNAMESPACE );

            iterator = navigator.Select( elementXPath , namespaceResolver );

            foreach ( XPathNavigator node in iterator )
            {
                recordHandler.Invoke( node , namespaceResolver );
            }

            return iterator.Count == rowsPerPage;
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
        private static Track[] GetTracksFromDB()
        {
            using ( F1Entities db = new F1Entities() )
            {
                return db.Tracks.ToArray();
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
                        if ( !db.Tracks.Any( t => t.Name == track.Name ) )
                        {
                            track.CountryId = country.CountryId;
                            track.Country = null;
                            db.Tracks.Add( track );
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Extracts races/grand prix data.
        /// </summary>
        public void ExtractRaces()
        {
            Dictionary<string , List<Race>> racesByTrack;
            Dictionary<string , Track> tracks;
            Dictionary<string , Country> countries;

            int pageNumber;
            Track track;

            racesByTrack = new Dictionary<string , List<Race>>();
            tracks = GetTracksFromDB().ToDictionary( t => t.Name , t => t );
            countries = GetCountriesFromDB().ToDictionary( c => c.Name , c => c );

            pageNumber = 0;
            while ( PopulateRacesFromPage( racesByTrack , pageNumber ) )
            {
                pageNumber++;
            }

            foreach ( string trackName in racesByTrack.Keys )
            {
                if ( tracks.ContainsKey( trackName ) )
                {
                    track = tracks[ trackName ];

                    AddRacesToDB( track , racesByTrack[ trackName ] , countries );
                }
            }
        }

        /// <summary>
        /// Adds new tracks to the database.
        /// </summary>
        /// <param name="track">Track at which the races occured.</param>
        /// <param name="races">Races to add.</param>
        /// <param name="countries">Dictionary for countries lookup.</param>
        private void AddRacesToDB( Track track , IEnumerable<Race> races , Dictionary<string , Country> countries )
        {
            GrandPrix existingGP;

            if ( races != null )
            {
                using ( F1Entities db = new F1Entities() )
                {
                    foreach ( Race race in races )
                    {
                        race.GrandPrix.CountryId = countries[ race.GrandPrix.Country.Name ].CountryId;//TODO:MF:Check if country always exists here.
                        existingGP = db.GrandPrixes.SingleOrDefault(
                            gp => gp.Name == race.GrandPrix.Name &&
                            gp.Country.Name == race.GrandPrix.Country.Name );

                        if ( existingGP == null )
                        {
                            race.GrandPrix.Country = null;
                            db.GrandPrixes.Add( race.GrandPrix );
                            db.SaveChanges();
                            race.GrandPrixId = race.GrandPrix.GrandPrixId;
                        }
                        else
                        {
                            race.GrandPrixId = existingGP.GrandPrixId;
                        }
                        
                        if ( !db.Races.Any( r => r.TrackId == track.TrackId && r.Season == race.Season ) )
                        {
                            race.TrackId = track.TrackId;
                            race.Track = null;
                            race.GrandPrix = null;
                            db.Races.Add( race );
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        //2.Ergast extraction code.
        //3.Azure Data Factory.
        //4.Cleansing: British Grand Prix vs British GP

        #endregion

    }
}
