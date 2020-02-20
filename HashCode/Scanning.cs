using System.Collections.Generic;
using System.Linq;

namespace HashCode {

    class Scanning {

        public void Process(List<Library> librariesList, int days) {

            var orderedLibraries = librariesList.OrderBy(library => library.Pointer).ToList();

            foreach ( Library library in orderedLibraries ) {
                
            }

        }

    }
}