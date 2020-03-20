using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode {

    class Scanning {

        public List<int> AllScannedBooks { get; set; } = new List<int>();

        public void Process( List<Library> librariesList, int days, Dictionary<int, int> orderedBookScores, string resultPath ) {

            // Ustawiamy biblioteki w kolejności od najwyższego Pointera
            List<Library> orderedLibraries = librariesList.OrderBy( library => -library.Pointer ).ToList();

            // bool flag = true;
            // while ( flag ) {
            //     flag = false;
            //Dla każdej biblioteki sprawdzamy czy książki do skanowania nie pojawiły się w zaagregowanej tabeli wszystkich książek (distinct) z poprzednich bibliotek. Jeśli jakaś książka z danej biblioteki pojawia się w aktualnej bibliotece to ją usuwamy - jeżeli wszystkie ksiązki z aktualnej biblioteki zostały usunięte to usuwamy bibliotekę, ponieważ skanowanie książek będzie bezpunktowe.
            orderedLibraries.ForEach( library => {
                var tempList = new List<int>();
                orderedLibraries.Take( orderedLibraries.IndexOf( library ) ).ToList().ForEach( library1 =>
                         tempList.AddRange( library1.BooksByScore.ToList().Distinct() ) );
                library.BooksByScore = library.BooksByScore.Where( i => !tempList.Contains( i ) ).ToArray();
            } );

            //Dla każdej biblioteki sprawdzamy czy nie usuneliśmy jakiejś książki, a jeśli usunęliśmy i są jakieś książki w tablicy usuniętych z etapu pierwszego obliczania danych( Library.CalculateValues() ) to dodajemy. Usuwamy wszystkie biblioteki, które nie mają książek do zeskanowania. Na nowo obliczamy wskaźnik - mógł się zmienić.
            orderedLibraries.ForEach( library => {
                if ( library.BooksByScore.Length == library.BooksByScoreLength || library.DeletedBooks == null ) return;
                int difference = library.BooksByScoreLength - library.BooksByScore.Length;
                var tempList = library.BooksByScore.ToList();
                int deletedBooksCount = difference >= library.DeletedBooks.Count ? library.DeletedBooks.Count : difference;
                var tempDeletedBooks = library.DeletedBooks.Where( ( i, i1 ) => i1 >= deletedBooksCount ).ToList();
                tempList.AddRange( library.DeletedBooks.Take( deletedBooksCount ) );
                library.DeletedBooks = tempDeletedBooks;
                library.BooksByScore = tempList.ToArray();
                library.BooksByScoreLength = library.BooksByScore.Length;
                // if ( !flag ) {
                //     flag = true;
                // }
            } );
            orderedLibraries = orderedLibraries.Where( library => library.BooksByScore.Length > 0 ).ToList();
            orderedLibraries.ForEach( library => library.RecalculateValues( orderedBookScores, days ) );
            orderedLibraries = orderedLibraries.OrderBy( library => -library.Pointer ).ToList();
            // }

            // Jeśli suma dni rejestracji bibliotek >= liczbie wszystkich dni, to wiemy, że ostatnia biblioteka nie zeskanuje żadnej książki, tak więc szukamy jakiejś co zdobyła by dla nas kilka dodatkowych punktów i podmieniamy, nawet jeśli miała by zeskanować jedną książke
            int daysSum = 0;
            foreach ( var library in orderedLibraries ) {
                daysSum += library.SignupProcessDays;

                if ( daysSum < days ) continue;

                if ( library == orderedLibraries.Last() ) {
                    break;
                }

                var tempList = orderedLibraries.Take( orderedLibraries.IndexOf( library ) + 1 ).ToList();
                tempList.Remove( library );
                int daysLeft = library.SignupProcessDays - daysSum + days;

                orderedLibraries.RemoveRange( 0, orderedLibraries.IndexOf( library ) + 1 );

                var validLibraries = orderedLibraries.Where( library1 => library1.SignupProcessDays < daysLeft ).ToList();
                if ( validLibraries.Count > 0 ) {
                    foreach ( var validLibrary in validLibraries ) {
                        validLibrary.RecalculateValues( orderedBookScores, daysLeft );
                    }
                } else {
                    orderedLibraries = tempList;
                    break;
                }

                Library validLibrariesFirst = validLibraries.OrderBy( library1 => -library1.Pointer ).ToList().First();
                tempList.Add( validLibrariesFirst );
                orderedLibraries = tempList;
                break;

            }

            //Przypisujemy pierwszy i będziemy zwiększać w pętli, żeby od razu nie iterować po wszystkich posortowanych bibliotekach
            var orderedLibrariesCounter = orderedLibraries.Count;
            List<Library> copiedOrderedLibraries = new List<Library>() {
                orderedLibraries.First()
            };
            orderedLibrariesCounter--;

            //Proces Skanowania
            for ( int i = 0; i <= days; i++ ) {

                foreach ( Library library in copiedOrderedLibraries ) {

                    Library actualSignUpLibrary = copiedOrderedLibraries.FirstOrDefault( orderedLibrary => orderedLibrary.IsSignupProcessing );

                    //Przypisujemy tylko niezeskanowane książki
                    library.BooksByScore = library.BooksByScore.Where( i1 => !AllScannedBooks.Contains( i1 ) ).ToArray();

                    if ( library.SignupProcessDays == 0 && library.BooksByScore.Length > 0 || ( ( actualSignUpLibrary?.Equals( library ) ?? true ) && library.SignupProcess() ) ) {
                        //Skanujemy i przypisujemy zeskanowane do listy wszystkich zeskanowanych
                        AllScannedBooks.AddRange( library.ScanningProcess() );
                    }

                }

                var firstOfCopiedOrderedLibraries = copiedOrderedLibraries.FirstOrDefault();

                if ( firstOfCopiedOrderedLibraries != null && firstOfCopiedOrderedLibraries.SignupProcessDays == 0 && firstOfCopiedOrderedLibraries.BooksByScore.Length == 0 ) {
                    copiedOrderedLibraries.Remove( firstOfCopiedOrderedLibraries );
                    if ( copiedOrderedLibraries.Count == 0 )
                        break;
                }

                //Jeśli w ostatniej bibliotece trwa rejestracja to nie dodajemy kolejnych
                if ( copiedOrderedLibraries.Last().IsSignupProcessing ) continue;

                //Kolejny element do zainsertowania
                var firstElementToInsert = orderedLibraries.ElementAtOrDefault( orderedLibraries.Count - orderedLibrariesCounter );

                //Jeśli nie ma pierwszego elementu to nie dodajemy
                if ( firstElementToInsert == null ) continue;

                //Insertujemy element do listy i podnosimy licznik
                copiedOrderedLibraries.Add( orderedLibraries[orderedLibraries.Count - orderedLibrariesCounter] );
                orderedLibrariesCounter--;

            }

            var countScannedLibraries = orderedLibraries.Count( library => library.ScannedBooks.Count > 0 );

            string result = $"{countScannedLibraries}\n";

            var librariesThatScannedBooks = orderedLibraries.Where( ( library ) => library.ScannedBooks.Count > 0 );

            result += librariesThatScannedBooks.Aggregate( "", ( current, library ) => current + library.LibraryId + " " + library.ScannedBooks.Count + "\n" + library.ScannedBooks.Aggregate( "", ( s, bookId ) => s + bookId + " " ) + "\n" );

            File.WriteAllText( resultPath, result.Trim() );

        }

    }
}