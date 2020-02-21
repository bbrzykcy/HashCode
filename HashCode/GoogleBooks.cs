using HashCode.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode {

    class GoogleBooks {

        static void Main( string[] args ) {

            Dictionary<string, string> dataSets = new Dictionary<string, string> {
                {"a_example.txt", Resources.a_example },
                {"b_read_on.txt", Resources.b_read_on },
                {"c_incunabula.txt", Resources.c_incunabula },
                {"d_tough_choices.txt", Resources.d_tough_choices },
                {"e_so_many_books.txt", Resources.e_so_many_books },
                {"f_libraries_of_the_world.txt", Resources.f_libraries_of_the_world }
            };

            foreach ( var dataSet in dataSets ) {

                var splitted = dataSet.Value.Split( '\n' );
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
                        SignupProcessDays = int.Parse( splitted[i].Split( ' ' )[1] ),
                        BooksPerDay = int.Parse( splitted[i].Split( ' ' )[2] ),
                        ScannedBooks = new List<int>()
                    }
                    );
                    libraries.Last().CreateValues( bookScores );
                    counter++;
                }

                Scanning scanning = new Scanning();
                string resultPath = $"{Path.Combine( Directory.GetCurrentDirectory(), dataSet.Key )}";
                scanning.Process( libraries, days, resultPath );

            }

        }

    }
}
