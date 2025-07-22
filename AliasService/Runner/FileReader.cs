namespace AliasService.Runner;

public class FileReader<T>(string path, Func<string, T> lineTransformer){
    public IEnumerable<T> ReadFile()
    {
        StreamReader streamReader = new StreamReader(path);
        
        while (!streamReader.EndOfStream)
        {
            var line = streamReader.ReadLine();
            if (line != null)
            {
                yield return lineTransformer.Invoke(line);
            }
        }
        
        streamReader.Close();
    }
        
}