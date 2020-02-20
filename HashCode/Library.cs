using System.Collections.Generic;
using System.Linq;

namespace HashCode {

    class Library {

        public int[] Books { get; set; }
        public int[] BooksByScore { get; set; }
        public int SignupProcess { get; set; }
        public int BooksPerDay { get; set; }
        public long PossibleMaxScore { get; set; }
        public double Pointer { get; set; }

        public void CreateValues( Dictionary<int, int> bookScores ) {
            var orderedBookScores = bookScores.OrderBy( pair => -pair.Value )
                .ToDictionary( pair => pair.Key, pair => pair.Value );
            for ( int i = 0; i < orderedBookScores.Count; i++ ) {
                if ( Books.Contains( i ) ) {
                    PossibleMaxScore += orderedBookScores[i];
                }
            }
            BooksByScore = orderedBookScores.Where( pair => Books.Contains( pair.Key ) ).ToDictionary( pair => pair.Key, pair => pair.Value ).Values.ToArray();
            Pointer = PossibleMaxScore * BooksPerDay * SignupProcess / ( double )Books.Length;
        }

    }

}