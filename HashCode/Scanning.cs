using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode {

    class Scanning {

        private static string PATH = $"{Path.Combine( Directory.GetCurrentDirectory(), "d_tough_choices.txt" )}";

        public void Process( List<Library> librariesList, int days ) {

            string result;

            var orderedLibraries = librariesList.OrderBy( library => library.Pointer ).ToList();

            for ( int i = 0; i <= days; i++ ) {
                foreach ( Library library in orderedLibraries ) {

                    Library signUpingLibrary = orderedLibraries.FirstOrDefault( orderedLibrary => orderedLibrary.IsSignupProcessing );

                    if ( library.SignupProcessDays == 0 && library.Books.Length > 0 || ( ( signUpingLibrary?.Equals( library ) ?? true ) && library.SignupProcess() ) ) {
                        library.ScanningProcess();
                    }

                }
            }

            var countScannedLibraries = orderedLibraries.Count( library => library.ScannedBooks.Count > 0 );


            result = $"{countScannedLibraries}\n";

            var librariesThatScannedBooks = orderedLibraries.Where( ( library ) => library.ScannedBooks.Count > 0 );

            result += librariesThatScannedBooks.Aggregate( "", ( current, library ) => current + library.LibraryId + " " + library.ScannedBooks.Count + "\n" + library.ScannedBooks.Aggregate( "", ( s, bookId ) => s + bookId + " " ) + "\n" );


            File.WriteAllText( PATH, result.Trim() );

        }

        // void SignupProcess( Library library ) {
        //     if ( library.SignupProcessDays > 0 ) {
        //         library.SignupProcessDays -= 1;
        //         return;
        //     }
        //
        //     Scanning
        //
        // }

    }
}