using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace mockAPI.Middleware
{
    public class DatabasePerformanceHealthCheck : IHealthCheck
    {
        private IDbConnection dbConnection;
        private ILogger<DatabasePerformanceHealthCheck> _logger;

        private IOptionsMonitor<DatabasePerformanceOptions> _options;

        public DatabasePerformanceHealthCheck(IDbConnection connection, ILogger<DatabasePerformanceHealthCheck> logger, IOptionsMonitor<DatabasePerformanceOptions> options)
        {
            dbConnection = connection;
            _logger = logger;
            _options = options;

        }
 

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
              var optionsSnapshot = _options.Get(context.Registration.Name);
            var data = new Dictionary<string, object>();

              try
            {
                var stopwatch = Stopwatch.StartNew();
                
                dbConnection.Open();

                using var command = dbConnection.CreateCommand();
                command.CommandText = optionsSnapshot.TestQuery;
                command.CommandTimeout = optionsSnapshot.QueryTimeoutThreshold / 1000; 

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var recordCount = reader.GetInt32(0);
                    data.Add("RecordCount", recordCount);
                }

                stopwatch.Stop();
                var elapsed = stopwatch.ElapsedMilliseconds;

                data.Add("QueryExecutionTime", elapsed);
                data.Add("TestQuery", optionsSnapshot.TestQuery);

                if (elapsed < optionsSnapshot.DegradedThreshold)
                {
                    return Task.FromResult(HealthCheckResult.Healthy(
                        $"Database query completed in {elapsed}ms",
                        data));
                }
                else if (elapsed < optionsSnapshot.QueryTimeoutThreshold)
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"Database query took {elapsed}ms, which is slower than expected",
                        null));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"Database query took {elapsed}ms, indicating severe performance issues",
                        null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                data.Add("ExceptionMessage", ex.Message);
                data.Add("ExceptionStackTrace", ex.StackTrace!);
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Database query failed: {ex.Message}",
                    exception: ex,
                    data: data));
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }
    }
}