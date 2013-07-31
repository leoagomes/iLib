iLib
====

A Library for working with the Apple's iDevice MediaLibrary.sqlitedb.

iLib is released under the GNU General Public License (GPL). Check LICENSE for more.

This code: is a little messy, doesn't have the best documentation, maybe does not follow the best practices in the .NET world, but my excuse is:
I developed most of it around the morning of a day that I haven't slept the night before.
"iLib" is not a great name, I know. I am not good at picking names.

*DO NOT forget collect your SQLite-related garbage (.Dispose(), =null) or else you'll get some SIGSEGVs for christmas.

As the lib was developed on a Mac, through Mono/MonoDevelop/Xamarin Studios, and modifications to libsqlite3.dylib are not so nice (I tried, which ended up rendering my Mac useless until I fixed it), Mono.Data.Sqlite was used.
To adapt the Library so it'll work on Windows: comment out "using Mono.Data..." and uncomment "using System.Data.SQLite", then capitalize SQL in the types "SQLiteConnection", "SQLiteCommand" and "SQLiteReader". Don't forget about the System.Data.SQLite.dll.

ABOUT THE MediaFile OBJECT {

It was mainly based on the properties of a TagLib# Tag.
Right after that there were some properties I found while trying to understand the MediaLibrary DB. All of that properties can be found inside (new MediaFile).iTunes.
I am pretty sure the names are self explanatory, except for:

"Location", which is actually the name of the file (e.g. ADZQ.mp3).
"LocationKind", which is more of a kind then a location. Well, it is the kind of the file in a human-readable way.
"BaseLocation", which is the folder (inside an iDevice) where the file ("Location") is stored.

}

ABOUT THE iLibraryReader {

Open/CloseSQLite() -> Opens/Closes (and disposes and sets to null) the SQLConnection;
GetAllMediaFiles() -> foreach row in the 'item_extra' table, calls GetMediaFile (row["location"]). Returns a List<MediaFile> of the elements;
GetMediaFile(location) -> returns a MediaFile object based on it "location" (actually the name of the file e.g. AHDS.mp3);
GetDataTable(query,oc) -> returns a DataTable object based on a query (that should ONLY READ). OC means Open/Close... if you want the method to Open/Close connection or if you'll DIY.
And Again:
* DO NOT forget collect your SQLite-related garbage (.Dispose(), =null) or else you'll get some SIGSEGVs for christmas.

}

ABOUT APPLE'S MEDIALIBRARY.SQLITEDB {

I see you're still interested in my project and want to learn more about apple's MediaLibrary.sqlitedb.

In the future (just not now), I'll post something about it.

However, you can easily undertand the Database buy using sqlite3 command line tool and the commands ".tables", ".schema <table>" and "SELECT ..." statements.
}

*IF YOU LIKE THIS PROJECT AND ARE A DEV (WHICH YOU PROBABLY ARE), PLEASE CONTRIBUTE.
