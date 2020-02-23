using System.Collections.Generic;
using System.Linq;

namespace HashCode {

    class Library {

        public int LibraryId { get; set; }
        public int[] Books { get; set; }
        public int[] BooksByScore { get; set; }
        public int BooksByScoreLength { get; set; }
        public List<int> DeletedBooks { get; set; }
        public List<int> ScannedBooks { get; set; } = new List<int>();
        public int SignupProcessDays { get; set; }
        public int BooksPerDay { get; set; }
        public long PossibleMaxScore { get; set; }
        public double Pointer { get; set; }
        public bool IsSignupProcessing { get; set; }

        public void CreateValues( Dictionary<int, int> orderedBookScores, int days ) {

            long maxPossibleBooks = days * ( long )BooksPerDay - SignupProcessDays * ( long )BooksPerDay;
            if ( maxPossibleBooks > Books.Length ) {
                maxPossibleBooks = Books.Length;
            }
            var matchedBooks = orderedBookScores.Where( pair => Books.Contains( pair.Key ) );
            var keyValuePairs = matchedBooks.ToList();
            PossibleMaxScore = keyValuePairs.Take( keyValuePairs.Count >= ( int )maxPossibleBooks ? ( int )maxPossibleBooks : keyValuePairs.Count ).Sum( pair => pair.Value );

            BooksByScore = orderedBookScores.Where( pair => Books.Contains( pair.Key ) ).ToDictionary( pair => pair.Key, pair => pair.Value ).Keys.ToArray();

            if ( BooksByScore.Length >= maxPossibleBooks ) {
                DeletedBooks = BooksByScore.Where( ( i, i1 ) => i1 >= maxPossibleBooks ).ToList();
                BooksByScore = BooksByScore.Take( ( int )maxPossibleBooks ).ToArray();
            }
            BooksByScoreLength = BooksByScore.Length;

            Pointer = PossibleMaxScore / ( double )SignupProcessDays;

        }

        public void RecalculateValues( Dictionary<int, int> orderedBookScores, int daysLeft ) {

            long maxPossibleBooks = daysLeft * ( long )BooksPerDay - SignupProcessDays * ( long )BooksPerDay;
            if ( maxPossibleBooks > Books.Length ) {
                maxPossibleBooks = Books.Length;
            }

            var matchedBooks = orderedBookScores.Where( pair => BooksByScore.Contains( pair.Key ) );
            var keyValuePairs = matchedBooks.ToList();
            PossibleMaxScore = keyValuePairs.Take( keyValuePairs.Count >= ( int )maxPossibleBooks ? ( int )maxPossibleBooks : keyValuePairs.Count ).Sum( pair => pair.Value );

            Pointer = PossibleMaxScore / ( double )SignupProcessDays;

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