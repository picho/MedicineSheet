using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MedicineSheet
{
    public static class RowsManager
    {
        public static List<ExpiredMedicine> GetExpiredMedicine(IList<IList<Object>> rows) 
        {
            List<ExpiredMedicine> expiredMedicine = new List<ExpiredMedicine>();
            DateTime currentTime = DateTime.Now;

            int rowsPivot = 2;

            foreach(var item in rows) 
            {
                if(item[0] != null && item[3] != null) 
                {
                    DateTime medicineDate = DateTime.ParseExact(item[3].ToString(), "MMM-yyyy", new CultureInfo("es-ES")); 
                    bool wasNotified = item[4].ToString().Equals(NotifiedValues.Yes.ToString());  
                    
                    if(currentTime > medicineDate && !wasNotified) 
                        expiredMedicine.Add(new ExpiredMedicine(item[0].ToString(), medicineDate, rowsPivot));
                }

                rowsPivot++;
            }

            return expiredMedicine;
        }
        
    }
}