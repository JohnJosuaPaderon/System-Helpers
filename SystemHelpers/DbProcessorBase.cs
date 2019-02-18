using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public abstract class DbProcessorBase : IDisposable
    {
        public DbProcessorBase(string connectionString, bool wrapInTransaction = false)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is required.", nameof(connectionString));
            }
            
            Connection = new SqlConnection(connectionString);
            WrapInTransaction = wrapInTransaction;
        }
        
        protected SqlConnection Connection { get; }
        protected bool WrapInTransaction { get; }
        protected SqlTransaction Transaction { get; set; }
        protected bool Faulted { get; set; }

        public virtual void Dispose()
        {
            FinalizeTransaction();
            FinalizeConnection();
        }

        private void FinalizeConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }

        private void FinalizeTransaction()
        {
            if (Transaction != null && WrapInTransaction)
            {
                if (Faulted)
                {
                    Transaction?.Rollback();
                }
                else
                {
                    Transaction?.Commit();
                }
            }
        }
    }

    public delegate SqlCommand CreateCommand(SqlConnection connection, SqlTransaction transaction);
    public delegate T ExecuteNonQueryCallback<T>(int affectedRows, SqlCommand command);
    public delegate T ConvertScalarValue<T>(object value);
    public delegate T ConvertReader<T>(SqlDataReader reader);
    public delegate Task<T> ConvertReaderAsync<T>(SqlDataReader reader, CancellationToken cancellationToken = default(CancellationToken));
}
