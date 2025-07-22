using System.Diagnostics;
using System.Text.RegularExpressions;
using AliasService.Utils;

namespace AliasService.Runner;

public class AliasRunner
{

    private readonly string _nonUrlPattern = @"^[A-Z]:\\";
    
    private Dictionary<string, string> _aliases;
    private const char Splitter = '=';

    public AliasRunner()
    {
        var filePath = Application.StartupPath + @"\Alias.txt";

       var reader = new FileReader<Alias>(filePath, LineTransformer);

       try
       {
          var aliases = reader.ReadFile();
          _aliases = new Dictionary<string, string>();
          aliases.ForEach(a => _aliases.Add(a.Name, a.Path));
       }
       catch (Exception _)
       {
           throw new Exception("Error reading alias file");
       }
    }

    public bool RunAlias(string alias)
    {
        if (_aliases.TryGetValue(alias, out var exe))
        {
            var regex = new Regex(_nonUrlPattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(exe))
            {
                Process.Start(exe);
                return true;
            }

            var psi = new ProcessStartInfo
            {
                FileName = exe,
                UseShellExecute = true
            };

            Process.Start(psi);
            return true;
        }
        
        return false;
    }
    
    

    private Alias LineTransformer(string line)
    {
        var parts = line.Split(Splitter);

        if (parts.Length < 2)
        {
            throw new Exception("Invalid line format");
        }

        if (parts.Length > 2)
        {
            var aliasExe = string.Join(Splitter, parts.Skip(1));
            
            return new Alias(parts[0], aliasExe);

        }
        
        return new Alias(parts[0], parts[1]);
    }
}