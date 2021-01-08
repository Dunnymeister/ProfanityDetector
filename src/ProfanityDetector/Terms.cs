using System.Collections.Generic;

namespace Ebooks.ProfanityDetector
{
    public class Terms
    {
        public Terms()
        {
            Prohibited = new HashSet<string>();
            Permitted = new HashSet<string>();
        }

        public HashSet<string> Prohibited { get; set; }
        public HashSet<string> Permitted { get; set; }
    }
}
