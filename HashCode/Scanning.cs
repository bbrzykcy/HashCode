using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode {

    class Scanning {

        public List<int> AllScannedBooks { get; set; }

        public void Process( List<Library> librariesList, int days, string resultPath ) {
            AllScannedBooks = new List<int>();
            string result;

            var orderedLibraries = librariesList.OrderBy( library => library.Pointer ).ToList();
            var orderedLibrariesCounter = orderedLibraries.Count;
            List<Library> copiedOrderedLibraries = new List<Library>() {
                orderedLibraries.First() //Przypisujemy pierwszy i będziemy zwiększać w pętli, żeby od razu nie iterować po wszystkich posortowanych bibliotekach
            };
            orderedLibrariesCounter--;
            for ( int i = 0; i <= days; i++ ) {

                foreach ( Library library in copiedOrderedLibraries ) {

                    Library signUpingLibrary = copiedOrderedLibraries.FirstOrDefault( orderedLibrary => orderedLibrary.IsSignupProcessing );

                    library.BooksByScore = library.BooksByScore.Where( i1 => !AllScannedBooks.Contains( i1 ) ).ToArray(); //Przypisujemy tylko niezeskanowane książki
                    if ( library.SignupProcessDays == 0 && library.BooksByScore.Length > 0 || ( ( signUpingLibrary?.Equals( library ) ?? true ) && library.SignupProcess() ) ) {
                        AllScannedBooks.AddRange( library.ScanningProcess( AllScannedBooks ) ); //Skanujemy i przypisujemy zeskanowane do listy wszystkich zeskanowanych
                    }

                }

                var firstOfCopiedOrderedLibraries = copiedOrderedLibraries.FirstOrDefault();

                if ( firstOfCopiedOrderedLibraries != null && firstOfCopiedOrderedLibraries.SignupProcessDays == 0 && firstOfCopiedOrderedLibraries.BooksByScore.Length == 0 ) {
                    copiedOrderedLibraries.Remove( firstOfCopiedOrderedLibraries );
                    if ( copiedOrderedLibraries.Count == 0 )
                        break;
                }

                if ( copiedOrderedLibraries.Last().IsSignupProcessing ) continue; //Jeśli w ostatniej bibliotece trwa rejestracja to nie dodajemy kolejnych
                var firstElementToInsert = orderedLibraries.ElementAtOrDefault( orderedLibraries.Count - orderedLibrariesCounter ); //Kolejny element do zainsertowania
                if ( firstElementToInsert == null ) continue;
                copiedOrderedLibraries.Add( orderedLibraries[orderedLibraries.Count - orderedLibrariesCounter] ); //Insertujemy element do listy
                orderedLibrariesCounter--;
            }

            var countScannedLibraries = orderedLibraries.Count( library => library.ScannedBooks.Count > 0 );

            result = $"{countScannedLibraries}\n";

            var librariesThatScannedBooks = orderedLibraries.Where( ( library ) => library.ScannedBooks.Count > 0 );

            result += librariesThatScannedBooks.Aggregate( "", ( current, library ) => current + library.LibraryId + " " + library.ScannedBooks.Count + "\n" + library.ScannedBooks.Aggregate( "", ( s, bookId ) => s + bookId + " " ) + "\n" );

            File.WriteAllText( resultPath, result.Trim() );

        }

    }
}