using PFS_BIP.Data;
using PFS_BIP.Models;
using PFS_BIP.Repository.Abstract;

namespace PFS_BIP.Repository.Implementation
{
    public class PuppiesRepo : IPuppiesRepo
    {
        private readonly PFSDbContext _context;
        public PuppiesRepo(PFSDbContext context)
        {
            this._context=context;
        }
        public bool Add(Puppies model)
        {
            try
            {
                _context.Puppies.Add(model);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex) {

                return false;
            }
            
        }
    }
}
