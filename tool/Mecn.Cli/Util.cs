namespace Mecn.Cli;

internal static class Util
{
    private const string MissingConnStr = "Missing database connection string. Either provide it explicitly by using " +
                                          "commandline argument -c or --connection-string or (using same argument) " +
                                          "pass name of environment variable containing connection string. " +
                                          "The default is MECN_CONNECTION_STRING.";

    internal static string NormalizeConnectionString(string connStr)
    {
        if (!string.IsNullOrEmpty(connStr) && connStr.Contains("Database="))
        {
            return connStr;
        }

        var csEnv = Environment.GetEnvironmentVariable(connStr);
        if (!string.IsNullOrEmpty(csEnv) && csEnv.Contains("Database="))
        {
            return csEnv;
        }

        throw new ArgumentException(MissingConnStr, nameof(connStr));
    }

    internal static Dictionary<string, object> ToHierarchy(Dictionary<string, string> data)
    {
        var result = new Dictionary<string, object>();
        foreach (var key in data.Keys.OrderByDescending(x => x))
        {
            var kparts = key.Split(':', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();
            if (kparts.Length == 1)
            {
                result[kparts[0]] = data[key];
            }
            else
            {
                var curr = result;
                for (var i = 0; i < kparts.Length - 1; i++)
                {
                    var currKey = kparts[i];
                    if (curr.TryGetValue(currKey, out var existing))
                    {
                        curr = (Dictionary<string, object>)existing;
                    }
                    else
                    {
                        var newDict = new Dictionary<string, object>();
                        curr[currKey] = newDict;
                        curr = newDict;
                    }
                }

                curr[kparts.Last()] = ToJsonCompatibleValue(data[key]);
            }
        }

        return result;
    }

    internal static object ToJsonCompatibleValue(string v)
    {
        var lcv = v.ToLowerInvariant();
        if (lcv == "true" || lcv == "false")
        {
            return bool.Parse(lcv);
        }

        if (double.TryParse(lcv, out var doubleVal))
        {
            return doubleVal;
        }

        return v;
    }
}
