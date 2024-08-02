using Microsoft.EntityFrameworkCore;
using ndisforms.Data.Models;

namespace ndisforms.Data.Providers
{
    public interface IIrDataProvider
    {
        IR_Header CreateNewIR(IR_Header IrToCreate);
        IR_Header GetIR(int IrId);
    }
    public class IrDataProvider : IIrDataProvider
    {
        private readonly NDISDbContext _context;

        public IrDataProvider(NDISDbContext context)
        {
            _context = context;
        }

        public IR_Header CreateNewIR(IR_Header IrToCreate)
        {
            if (IrToCreate != null)
            {
                string newIR = GetNextIrSequenceValue().ToString("00000");
                var newirnumber = "IR-" + newIR + "-" + DateTime.Today.Year.ToString();
                IrToCreate.Report_number = newirnumber;
                IrToCreate.RC_CR_DTM = DateTime.Now;
                _context.Add(IrToCreate);
                _context.SaveChanges();
            }

            return IrToCreate;
        }

        public IR_Header GetIR(int IrId)
        {
            return _context.IR_Header.Where(x => x.Id == IrId).FirstOrDefault();
        }


        public int GetNextIrSequenceValue()
        {
            int result = 0;
            var p = new Microsoft.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
            p.Direction = System.Data.ParameterDirection.Output;
            _context.Database.ExecuteSqlRaw("set @result = next value for SeqIR", p);
            var nextId = (int)p.Value;


            if (nextId > 0)
            {
                result = nextId;
            }

            return result;
        }

    }


}
