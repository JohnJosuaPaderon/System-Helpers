using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public sealed class AsyncDbProcessor : DbProcessorBase
    {
        public AsyncDbProcessor(string connectionString, bool wrapInTransaction = false) : base(connectionString, wrapInTransaction)
        {
        }

        public async Task<SqlConnection> GetConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync(cancellationToken);
            }

            return Connection;
        }

        public async Task<SqlTransaction> GetTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (WrapInTransaction)
            {
                if (Transaction == null)
                {
                    Transaction = (await GetConnectionAsync()).BeginTransaction();
                }

                return Transaction;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(SqlCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(CreateCommand createCommand, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteNonQueryAsync(command, cancellationToken);
            }
        }

        public async Task<T> ExecuteNonQueryAsync<T>(SqlCommand command, ExecuteNonQueryCallback<T> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return callback(await command.ExecuteNonQueryAsync(cancellationToken), command);
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<T> ExecuteNonQueryAsync<T>(CreateCommand createCommand, ExecuteNonQueryCallback<T> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteNonQueryAsync(command, callback, cancellationToken);
            }
        }

        public async Task<T> ExecuteReaderAsync<T>(SqlCommand command, ConvertReader<T> convertReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.Read())
                    {
                        return convertReader(reader);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<T> ExecuteReaderAsync<T>(CreateCommand createCommand, ConvertReader<T> convertReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteReaderAsync(command, convertReader, cancellationToken);
            }
        }

        public async Task<T> ExecuteReaderAsync<T>(SqlCommand command, ConvertReaderAsync<T> convertReaderAsync, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (await reader.ReadAsync(cancellationToken))
                    {
                        return await convertReaderAsync(reader, cancellationToken);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<T> ExecuteReaderAsync<T>(CreateCommand createCommand, ConvertReaderAsync<T> convertReaderAsync, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteReaderAsync(command, convertReaderAsync, cancellationToken);
            }
        }

        public async Task<IEnumerable<T>> ExecuteReaderListAsync<T>(SqlCommand command, ConvertReader<T> convertReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        var result = new List<T>();

                        while (await reader.ReadAsync(cancellationToken))
                        {
                            result.Add(convertReader(reader));
                        }

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<IEnumerable<T>> ExecuteReaderListAsync<T>(CreateCommand createCommand, ConvertReader<T> convertReader, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteReaderListAsync(command, convertReader, cancellationToken);
            }
        }

        public async Task<IEnumerable<T>> ExecuteReaderListAsync<T>(SqlCommand command, ConvertReaderAsync<T> convertReaderAsync, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        var result = new List<T>();

                        while (await reader.ReadAsync(cancellationToken))
                        {
                            result.Add(await convertReaderAsync(reader));
                        }

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<IEnumerable<T>> ExecuteReaderListAsync<T>(CreateCommand createCommand, ConvertReaderAsync<T> convertReaderAsync, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteReaderListAsync(command, convertReaderAsync, cancellationToken);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(SqlCommand command, ConvertScalarValue<T> convertScalarValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return convertScalarValue(await command.ExecuteScalarAsync(cancellationToken));
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(CreateCommand createCommand, ConvertScalarValue<T> convertScalarValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var command = createCommand(await GetConnectionAsync(cancellationToken), await GetTransactionAsync(cancellationToken)))
            {
                return await ExecuteScalarAsync(command, convertScalarValue, cancellationToken);
            }
        }
    }   
}
