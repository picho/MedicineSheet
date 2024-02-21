
namespace MedicineSheet
{
    public interface IGoogleSheetManager
    {
        IList<IList<Object>> GetMultipleValues();
        void UpdateMedicineStatus(IEnumerable<ExpiredMedicine> expiredMedicines);
    }
}