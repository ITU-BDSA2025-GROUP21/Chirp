using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimpleDB { 
    public interface IDatabaseRepository<T> { 
        public IEnumerable<T> Read(int? limit = null);
        public void Store(T record); 
    }
}