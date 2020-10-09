using System.Collections.Generic;

namespace Data.Entity
{
    public class EntityFilter<T>
    {
        public long Total { get; set; }
       
        public IEnumerable<T> DataList { get; set; }
    }
}
