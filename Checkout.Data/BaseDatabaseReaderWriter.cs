using Checkout.Data.Stubs;

namespace Checkout.Data
{
    public class BaseDatabaseReaderWriter
    {
        protected readonly Db db;
        public BaseDatabaseReaderWriter(Db db) 
        {
            this.db = db;
        }
    }
}
