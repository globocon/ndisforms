using ndisforms.Data.Models;
using ndisforms.Data.Providers;

namespace ndisforms.Data.Services
{
    public interface IViewDataService
    {
       IR_Header SaveNewIR(IR_Header IrToCreate);
     

        List<IR_Header> GetDataIncidentReports { get; }
    }

    public class ViewDataService : IViewDataService
    {
        private readonly IIrDataProvider _irDataProvider;
        public ViewDataService(IIrDataProvider irDataProvider)
        {           
            _irDataProvider = irDataProvider;
        }

        public IR_Header SaveNewIR(IR_Header IrToCreate)
        {
            return _irDataProvider.CreateNewIR(IrToCreate);
        }
        public List<IR_Header> GetDataIncidentReports
        {
            get
            {
                var IR_Header = _irDataProvider.GetIrs().ToList();
                return IR_Header;
            }
        }
    }
}
