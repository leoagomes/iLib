iLib
====

<p>A Library for working with the Apple's iDevice MediaLibrary.sqlitedb.<br/>
iLib is released under the GNU General Public License (GPL). Check LICENSE for more.</p>
If you like this project and know enough of C# to <b>contribute, please</b> do it!

<p>The current version of iLib only supports iOS 5 and 6. Support for iOS 3 is under development.</p>

<h3>About: iLib</h3>
<p>iLib's code is a little messy, is not as well documented as I wanted to and maybe does not follow the best practices in the .NET world. I'm sorry.<br/>

As the lib was developed on a Mac, through Mono, Mono.Data.Sqlite was used.<br/></p>

<h4>Adapting for Windows</h4>

<ol>
<li>Comment out "<code>using Mono.Data.Sqlite;</code>"</li>
<li>Uncomment "<code>using System.Data.SQLite;</code>"</li>
<li>Capitalize "SQLite" ("SQLiteConnection", "SQLiteCommand" and "SQLiteReader")</li>
</ol>

Don't forget about the System.Data.SQLite.dll.

<h3>About: MediaFile</h3>

<p>It was mainly based on the properties of a TagLib# Tag.<br/>
Right after that there were some properties I found while trying to understand the MediaLibrary DB. All of that properties can be found inside (new MediaFile).iTunes.<br/>
<br/>I am pretty sure the names are self explanatory, except for:</p>

<ul>
<li><code>Location</code>, which is actually the name of the file (e.g. ADZQ.mp3).</li>
<li><code>LocationKind</code>, which is more of a kind then a location. Well, it is the kind of the file in a human-readable way.</li>
<li><code>BaseLocation</code>, which is the folder (inside an iDevice) where the file ("Location") is stored.</li>
</ul>

<h3>About: iLibraryReader</h3>

<ul>
<li><code>Open/CloseSQLite()</code> -> Opens/Closes (and disposes and sets to null) the SQLConnection;</li>
<li><code>GetAllMediaFiles()</code> -> foreach row in the 'item_extra' table, calls GetMediaFile (row["location"]). Returns a List<MediaFile> of the elements;</li>
<li><code>GetMediaFile(location)</code> -> returns a MediaFile object based on it "location" (actually the name of the file e.g. AHDS.mp3);</li>
<li><code>GetDataTable(query,oc)</code> -> returns a DataTable object based on a query (that should ONLY READ). OC means Open/Close... if you want the method to Open/Close connection or if you'll DIY.</li>
</ul>

* DO NOT forget collect your SQLite-related garbage (.Dispose(), =null) or else you'll get some SIGSEGVs for christmas.

<h3>About: MediaLibrary.sqlitedb</h3>

<p>I see you're still interested in my project and want to learn more about apple's MediaLibrary.sqlitedb.
In the future (just not now), I'll post something about it.<br/><br/>
However, you can easily undertand the Database buy using sqlite3 command line tool and the commands ".tables", ".schema (table)" and "SELECT ..." statements.</p>
