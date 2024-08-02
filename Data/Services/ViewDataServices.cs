using ndisforms.Data.Models;
using ndisforms.Data.Providers;

namespace ndisforms.Data.Services
{
    public interface IViewDataService
    {
       IR_Header SaveNewIR(IR_Header IrToCreate);
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
    }
}
