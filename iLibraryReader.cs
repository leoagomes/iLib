using System;
using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite; //Sorry about this, but aparently System.Data.SQLite doesn't really like my Mac.
//using System.Data.SQLite;

namespace iLib
{
	public class iLibraryReader
	{
		private string dbConnString;
		private SqliteConnection sqlite;

		//Please notice that Mono.Data.Sqlite requires DIY Garbage Collection,
		//so when using SQLite-related stuff, do your .Dispose()s and set the obj to null

		public iLibraryReader (string pathToMediaLibrary)
		{
			dbConnString = string.Format("Data Source={0}", pathToMediaLibrary);
			sqlite = new SqliteConnection (dbConnString);
		}

		/// <summary>
		/// Opens the SQLite Connection.
		/// </summary>
		public void OpenSQLite() {
			if (sqlite == null) {
				sqlite = new SqliteConnection (dbConnString);
			}
			sqlite.Open ();
		}

		/// <summary>
		/// Closes the SQLite Connection.
		/// </summary>
		public void CloseSQLite() {
			sqlite.Close ();
			sqlite.Dispose ();
			sqlite = null;
		}

		/// <summary>
		/// Returns all the MediaFile objects it can find in the database.
		/// </summary>
		/// <returns>List of MediaFiles</returns>
		public List<MediaFile> GetAllMediaFiles() {
			List<MediaFile> list = new List<MediaFile>();
			string itemQuery = "SELECT location FROM item_extra";
			DataTable dt = GetDataTable (itemQuery, true);

			foreach (DataRow r in dt.Rows) {
				MediaFile mf = GetMediaFile((string)r["location"]);
				list.Add(mf);
			}

			return list;
		}

		/// <summary>
		/// Gets a single MediaFile object based on it's "location", which is actually only the name of the file.
		/// </summary>
		/// <returns>A MediaFile object</returns>
		/// <param name="location">Database calls it location, but is ONLY the name of the file. (i.e. WXYZ.mp3)</param>
		public MediaFile GetMediaFile (string location)
		{
			//OK, here's where the magic happens:
			//for a song, we have to find the Album,
			//the Artist and the Title, the Genre, the Duration, the Composer, etc.
			//We do this, by referencing to different tables inside the 'main' database from MediaLibrary.sqlitedb
			//For example: we rerfer to 'album' to get the album of a song. BUT FIRST...
			//WE NEED TO FIRST GET THE 'item_pid' of a song through table item_extra, which relates the track's title
			//)to it's 'item_pid'... then we use the 'item_pid' to get the other properties 'linkers' from the "item" table.
			//From there, we get the pids ('album_pid','album_artist_pid',etc.) and then we refer to the actual tables that
			//contain the data we want. If you understand a bit of SQL (like me) you'll understand the rest of the code.

			//This method fills up an MediaFile object, which means it may get much more data than you want.
			//Maybe sometimes it is easier to query the data using GetDataTable and parse the results yourself.

			if (sqlite == null) {
				sqlite = new SqliteConnection (dbConnString);
			}

			MediaFile file = new MediaFile ();
			sqlite.Open ();
			DataTable table = GetDataTable ("SELECT * FROM item_extra WHERE location = \"" + location + "\"", false);
			DataRow row = table.Rows [0];

			//uint location_type = Convert.ToUInt32(row ["location_type"]);

			file.iTunes.item_pid = (long)row ["item_pid"];
			file.iTunes.Location = row ["location"].ToString();
			file.iTunes.IsPodcast = Convert.ToBoolean(row ["is_podcast"]);
			file.iTunes.Description = row ["description"].ToString();
			file.iTunes.DescriptionLong = row ["description_long"].ToString();
			file.iTunes.IsOTAPurchased = Convert.ToBoolean(row ["is_ota_purchased"]);
			file.iTunes.IsAudioBook = Convert.ToBoolean(row ["is_audible_audio_book"]);
			file.iTunes.IsRental = Convert.ToBoolean(row ["is_rental"]);

			file.Title = row ["title"].ToString();
			file.TitleSort = row ["sort_title"].ToString();
			file.DiscCount = Convert.ToUInt32 (row ["disc_count"]);
			file.TrackCount = Convert.ToUInt32 (row ["track_count"]);
			file.Year = Convert.ToUInt32 (row ["year"]);
			file.BeatsPerMinute = Convert.ToUInt32 (row ["bpm"]);
			file.Comment = row ["comment"].ToString();
			file.Copyright = row ["copyright"].ToString();

			//now from item
			table = GetDataTable ("SELECT * FROM item WHERE item_pid = " + file.iTunes.item_pid, false);
			row = table.Rows [0];

			long item_artist_pid = (long)row ["item_artist_pid"];
			long album_pid = (long)row ["album_pid"];
			long album_artist_pid = (long)row ["album_artist_pid"];
			long composer_pid = (long)row ["composer_pid"];
			long genre_id = (long)row ["genre_id"];
			long location_kind_id = (long)row ["location_kind_id"];
			long category_id = (long)row ["category_id"];
			long base_location_id = (long)row ["base_location_id"];

			file.Track = Convert.ToUInt32 (row ["track_number"]);
			file.Disc = Convert.ToUInt32 (row ["disc_number"]);

			//now the artist
			if (item_artist_pid != 0) {
				table = GetDataTable ("SELECT * FROM item_artist WHERE item_artist_pid = " + item_artist_pid, false);
				row = table.Rows [0];

				file.Performers = row ["item_artist"].ToString();
				file.PerformersSort = row ["sort_item_artist"].ToString();
			} else {
				file.Performers = string.Empty;
				file.PerformersSort = string.Empty;
			}

			//now the album
			if (album_pid != 0) {
				table = GetDataTable ("SELECT * FROM album WHERE album_pid = " + album_pid, false);
				row = table.Rows [0];

				file.Album = row ["album"].ToString();
				file.AlbumSort = row ["sort_album"].ToString();
			} else {
				file.Album = string.Empty;
				file.AlbumSort = string.Empty;
			}

			//now the album artist
			if (album_artist_pid != 0) {
				table = GetDataTable ("SELECT * FROM album_artist WHERE album_artist_pid = " + album_artist_pid, false);
				row = table.Rows [0];

				file.AlbumArtist = row ["album_artist"].ToString();
				file.AlbumArtistSort = row ["sort_album_artist"].ToString();
			} else {
				file.AlbumArtist = string.Empty;
				file.AlbumArtistSort = string.Empty;
			}

			//now the composer
			if (composer_pid != 0) {
				table = GetDataTable ("SELECT * FROM composer WHERE composer_pid = " + composer_pid, false);
				row = table.Rows [0];

				file.Composers = row ["composer"].ToString();
				file.ComposersSort = row ["sort_composer"].ToString();
			} else {
				file.Composers = string.Empty;
				file.ComposersSort = string.Empty;
			}

			//now the genre
			if (genre_id != 0) {
				table = GetDataTable ("SELECT * FROM genre WHERE genre_id = " + genre_id, false);
				row = table.Rows [0];

				file.Genres = row ["genre"].ToString();
			} else {
				file.Genres = string.Empty;
			}

			//now the lyrics
			table = GetDataTable ("SELECT * FROM lyrics WHERE item_pid = " + file.iTunes.item_pid, false);
			if (table.Rows.Count > 0) {
				file.Lyrics = table.Rows [0] ["lyrics"].ToString();
			} else {
				file.Lyrics = string.Empty;
			}

			//some itunes stuff
			//user ratings
			table = GetDataTable ("SELECT * FROM item_stats WHERE item_pid = " + file.iTunes.item_pid, false);
			if (table.Rows.Count > 0) {
				row = table.Rows [0];
				file.iTunes.Rating = Convert.ToUInt32 (row ["user_rating"]);
				file.iTunes.PlayCount = Convert.ToUInt32 (row ["play_count_user"]);
				file.iTunes.SkipCount = Convert.ToUInt32 (row ["skip_count_user"]);
				file.iTunes.HasBeenPlayed = Convert.ToBoolean(row ["has_been_played"]);
				file.iTunes.IsDownloading = Convert.ToBoolean(row ["is_downloading"]);

			} else {
				file.iTunes.Rating = 0;
				file.iTunes.PlayCount = 0;
				file.iTunes.SkipCount = 0;
				file.iTunes.HasBeenPlayed = false;
				file.iTunes.IsDownloading = false;
			}

			//location kind
			table = GetDataTable ("SELECT * FROM location_kind WHERE location_kind_id = " + location_kind_id, false);
			row = table.Rows [0];
			file.iTunes.LocationKind = row ["kind"].ToString();

			//category
			if (category_id != 0) {
				table = GetDataTable ("SELECT * FROM category WHERE category_id = " + category_id, false);
				row = table.Rows [0];
				file.iTunes.Category = row ["category"].ToString ();
			} else {
				file.iTunes.Category = string.Empty;
			}

			//base location
			table = GetDataTable ("SELECT * FROM base_location WHERE base_location_id = " + base_location_id, false);
			row = table.Rows [0];
			file.iTunes.BaseLocation = row ["path"].ToString();

			sqlite.Close ();
			sqlite.Dispose ();
			sqlite = null;
			return file;
		}

		/// <summary>
		/// Runs the query in SQLite and returns the result as a DataTable.
		/// USE ONLY FOR READING (SELECT FROM).
		/// </summary>
		/// <returns>A DataTable containing the result.</returns>
		/// <param name="query">Query.</param>
		/// <param name="oc">If set to <c>true</c>, this will Open/Close SQLite Connection.</param>
		public DataTable GetDataTable(string query, bool oc) {
			DataTable table = new DataTable ();
			try 
			{
				if (sqlite == null) {
					sqlite = new SqliteConnection(dbConnString);
					oc = true;
				}
				if (oc)
					sqlite.Open ();
				SqliteCommand cmd = new SqliteCommand (query, sqlite);
				SqliteDataReader reader = cmd.ExecuteReader ();
				table.Load (reader);
				reader.Close ();
				reader = null;
				cmd.Dispose ();
				cmd = null;

				if (oc) {
					sqlite.Close();
					sqlite.Dispose();
					sqlite = null;
				}
		 	}
			catch (Exception e) {
				throw new Exception (e.Message);
			}
			return table;
		}
	}
}

