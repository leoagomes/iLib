using System;

namespace iLib
{
	public struct MediaFile
	{
		public struct _iTunesDB
		{
			public long item_pid;

			//String
			public string BaseLocation;
			public string Category;
			public string Location; //More of a file name.
			public string LocationKind; //More of a Kind, Actually
			public string StoreLink;
			public string Description;
			public string DescriptionLong;

			//Bool
			public bool ExcludeFromShuffle;
			public bool IsAudioBook;
			public bool IsOTAPurchased;
			public bool IsPodcast;
			public bool IsRental;
			public bool HasBeenPlayed;
			public bool IsDownloading;

			//integers
			public uint Rating;
			public uint PlayCount;
			public uint SkipCount;

		}

		public _iTunesDB iTunes;

		public string Album;
		public string AlbumSort;
		public string AlbumArtist;
		public string AlbumArtistSort;
		public uint   BeatsPerMinute;
		public string Comment;
		public string Composers;
		public string ComposersSort;
		public string Copyright;
		public uint   Disc;
		public uint   DiscCount;
		public string Genres;
		public string Lyrics;
		public string Performers;
		public string PerformersSort;
		public string Title;
		public string TitleSort;
		public uint   Track;
		public uint   TrackCount;
		public uint	  Year;
	}
}

