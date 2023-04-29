using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class ListsExtensions
    {
        public static bool IsNull<T>(this List<T> inList) => inList == null;
        
        public static bool IsEmpty<T>(this List<T> inList) => !inList.Any(); //exp : !inList.Any is so useful when the list isnt empty
        
        public static List<T> ResetList<T>(this List<T> inList)
        {
            if(inList.IsNull()) return new List<T>();
            if (inList.IsEmpty()) return inList;
            
            inList.Clear();
            return inList;
        }
        
    }
}
