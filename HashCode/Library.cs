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

            PossibleMaxScore = orderedBookScores.Where( pair => Books.Contains( pair.Key ) ).Sum( pair => pair.Value );

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

        public List<int> ScanningProcess( List<int> allScannedBooks ) {

            var result = new List<int>( BooksByScore.Length >= BooksPerDay
                ? BooksByScore.Take( BooksPerDay )
                : BooksByScore.Take( BooksByScore.Length ) );

            ScannedBooks.AddRange( BooksByScore.Length >= BooksPerDay
                ? BooksByScore.Take( BooksPerDay )
                : BooksByScore.Take( BooksByScore.Length ) );

            BooksByScore = BooksByScore.Skip( BooksByScore.Length >= BooksPerDay ? BooksPerDay : BooksByScore.Length ).ToArray();

            return result;
        }
    }

}