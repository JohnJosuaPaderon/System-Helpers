using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace System
{
    public sealed class DbProcessor : DbProcessorBase
    {
        public DbProcessor(string connectionString, bool wrapInTransaction = false) : base(connectionString, wrapInTransaction)
        {
        }

        public SqlConnection GetConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            return Connection;
        }

        public SqlTransaction GetTransaction()
        {
            if (WrapInTransaction)
            {
                if (Transaction == null)
                {
                    Transaction = GetConnection().BeginTransaction();
                }

                return Transaction;
            }
            else
            {
                return null;
            }
        }

        public int ExecuteNonQuery(SqlCommand command)
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public int ExecuteNonQuery(CreateCommand createCommand)
        {
            using (var command = createCommand(GetConnection(), GetTransaction()))
            {
                return ExecuteNonQuery(command);
            }
        }

        public T ExecuteNonQuery<T>(SqlCommand command, ExecuteNonQueryCallback<T> callback)
        {
            try
            {
                return callback(command.ExecuteNonQuery(), command);
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public T ExecuteNonQuery<T>(CreateCommand createCommand, ExecuteNonQueryCallback<T> callback)
        {
            using (var command = createCommand(GetConnection(), GetTransaction()))
            {
                return ExecuteNonQuery(command, callback);
            }
        }

        public T ExecuteReader<T>(SqlCommand command, ConvertReader<T> convertReader)
        {
            try
            {
                using (var reader = command.ExecuteReader())
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

        public T ExecuteReader<T>(CreateCommand createCommand, ConvertReader<T> convertReader)
        {
            using (var command = createCommand(GetConnection(), GetTransaction()))
            {
                return ExecuteReader(command, convertReader);
            }
        }

        public IEnumerable<T> ExecuteReaderList<T>(SqlCommand command, ConvertReader<T> convertReader)
        {
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var result = new List<T>();

                        while (reader.Read())
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

        public IEnumerable<T> ExecuteReaderList<T>(CreateCommand createCommand, ConvertReader<T> convertReader)
        {
            using (var command = createCommand(GetConnection(), GetTransaction()))
            {
                return ExecuteReaderList(command, convertReader);
            }
        }

        public T ExecuteScalar<T>(SqlCommand command, ConvertScalarValue<T> convertScalarValue)
        {
            try
            {
                return convertScalarValue(command.ExecuteScalar());
            }
            catch (Exception)
            {
                Faulted = true;
                throw;
            }
        }

        public T ExecuteScalar<T>(CreateCommand createCommand, ConvertScalarValue<T> convertScalarValue)
        {
            using (var command = createCommand(GetConnection(), GetTransaction()))
            {
                return ExecuteScalar(command, convertScalarValue);
            }
        }
    }
}
