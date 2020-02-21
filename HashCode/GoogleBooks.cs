using HashCode.Properties;
using System.Collections.Generic;
using System.Linq;

namespace HashCode {

    class GoogleBooks {

        static void Main( string[] args ) {

            string example = Resources.a_example;
            string b_read_on = Resources.b_read_on;
            string c_incunabula = Resources.c_incunabula;
            string d_tough_choices = Resources.d_tough_choices;
            string e_so_many_books = Resources.e_so_many_books;
            string f_libraries_of_the_world = Resources.f_libraries_of_the_world;

            var splitted = d_tough_choices.Split( '\n' );
            var books = int.Parse( splitted[0].Split( ' ' )[0] );
            var days = int.Parse( splitted[0].Split( ' ' )[2] );
            var scores = splitted[1].Split( ' ' ).Select( int.Parse ).ToArray();
            Dictionary<int, int> bookScores = new Dictionary<int, int>();

            for ( int i = 0; i < books; i++ ) {
                bookScores.Add( i, scores[i] );
            }
            List<Library> libraries = new List<Library>();
            int counter = 0;
            for ( int i = 2; i < splitted.Length - 2; i += 2 ) {
                libraries.Add( new Library {
                    LibraryId = counter, // Unikalny identyfikator
                    Books = splitted[i + 1].Split( ' ' ).Select( int.Parse ).ToArray(),
                    BooksPerDay = int.Parse( splitted[i].Split( ' ' )[1] ),
                    SignupProcessDays = int.Parse( splitted[i].Split( ' ' )[2] ),
                    ScannedBooks = new List<int>()
                }
                );
                libraries.Last().CreateValues( bookScores );
                counter++;
            }

            /*var query = from item in libraries
                        select new { s = $"{item.Books.Length} {item.BooksPerDay} {item.SignupProcessDays} Wskaźnik: {item.Pointer}" };

            foreach ( var item in query ) {
                Debug.WriteLine( item.s );
            }*/

            Scanning scanning = new Scanning();
            scanning.Process( libraries, days );


        }

    }
}
