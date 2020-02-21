using System.Collections.Generic;
using System.Linq;

namespace HashCode {

    class Library {

        public int LibraryId { get; set; }
        public int[] Books { get; set; }
        public int[] BooksByScore { get; set; }
        public List<int> ScannedBooks { get; set; }
        public int SignupProcessDays { get; set; }
        public int BooksPerDay { get; set; }
        public long PossibleMaxScore { get; set; }
        public double Pointer { get; set; }
        public bool IsSignupProcessing { get; set; }

        public void CreateValues( Dictionary<int, int> bookScores ) {

            var orderedBookScores = bookScores.OrderBy( pair => -pair.Value ).ToDictionary( pair => pair.Key, pair => pair.Value );

            for ( int i = 0; i < orderedBookScores.Count; i++ ) {
                if ( Books.Contains( i ) ) {
                    PossibleMaxScore += orderedBookScores[i];
                }
            }

            BooksByScore = orderedBookScores.Where( pair => Books.Contains( pair.Key ) ).ToDictionary( pair => pair.Key, pair => pair.Value ).Keys.ToArray();
            Pointer = PossibleMaxScore * BooksPerDay * SignupProcessDays / ( double )Books.Length;

        }

        public bool SignupProcess() {
            if ( SignupProcessDays == 0 ) {
                return false;
            }
            if ( IsSignupProcessing == false )
                IsSignupProcessing = true;

            SignupProcessDays -= 1;
            if ( SignupProcessDays == 0 ) {
                IsSignupProcessing = false;
            }
            return false;
        }

        public void ScanningProcess() {

            ScannedBooks.AddRange( BooksByScore.Length >= BooksPerDay
                ? BooksByScore.Take( BooksPerDay )
                : BooksByScore.Take( BooksByScore.Count() ) );

            BooksByScore = BooksByScore.Skip( BooksByScore.Count() >= BooksPerDay ? BooksPerDay : BooksByScore.Length ).ToArray();

        }
    }

}