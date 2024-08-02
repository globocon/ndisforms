using ndisforms.Data.Models;

namespace ndisforms.Data.Providers
{
    public interface IEmailDataProvider
    {
        List<TblEmail> GetEmailIds(string EmailType);
    }
    public class EmailDataProvider: IEmailDataProvider
    {
        private readonly NDISDbContext _context;

        public EmailDataProvider(NDISDbContext context)
        {
            _context = context;
        }

        public List<TblEmail> GetEmailIds(string EmailType)
        {
            return _context.TblEmail.Where(x => x.MailType.Equals(EmailType)).ToList();

        }

    }
}
