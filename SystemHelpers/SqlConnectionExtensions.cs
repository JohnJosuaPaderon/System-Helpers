using System.Data;
using System.Data.SqlClient;

namespace System
{
    public static class SqlConnectionExtensions
    {
        public static SqlCommand CreateQueryCommand(this SqlConnection instance, string commandText, SqlTransaction transaction)
        {
            var command = instance.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = commandText;

            return command;
        }

        public static SqlCommand CreateProcedureCommand(this SqlConnection instance, string procedure, SqlTransaction transaction)
        {
            var command = instance.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = procedure;
            command.CommandType = CommandType.StoredProcedure;

            return command;
        }
    }
}
