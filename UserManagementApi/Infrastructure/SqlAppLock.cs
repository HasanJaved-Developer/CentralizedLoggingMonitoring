using Microsoft.Data.SqlClient;
using System.Data;

namespace UserManagementApi.Infrastructure
{
    public static class SqlAppLock
    {
        public static async Task WithExclusiveLockAsync(
            string masterConnectionString,
            string resourceName,
            Func<CancellationToken, Task> action,
            int timeoutMs = 60000,
            CancellationToken ct = default)
        {
            await using var conn = new SqlConnection(masterConnectionString);
            await conn.OpenAsync(ct);

            // Acquire lock
            await using (var cmd = new SqlCommand("sp_getapplock", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Resource", resourceName);
                cmd.Parameters.AddWithValue("@LockMode", "Exclusive");
                cmd.Parameters.AddWithValue("@LockOwner", "Session");
                cmd.Parameters.AddWithValue("@LockTimeout", timeoutMs); // <-- correct name

                var ret = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;

                await cmd.ExecuteNonQueryAsync(ct);

                var code = (int)(ret.Value ?? -999);
                // 0 = success, 1 = already held by caller; negatives are failure
                if (code < 0)
                    throw new TimeoutException($"sp_getapplock failed ({code}) for '{resourceName}'.");
            }

            try
            {
                await action(ct);
            }
            finally
            {
                // Release lock
                await using var rel = new SqlCommand("sp_releaseapplock", conn) { CommandType = CommandType.StoredProcedure };
                rel.Parameters.AddWithValue("@Resource", resourceName);
                rel.Parameters.AddWithValue("@LockOwner", "Session");
                await rel.ExecuteNonQueryAsync(ct);
            }
        }
    }
}
