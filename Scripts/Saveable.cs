using System.Collections.Generic;

public interface Saveable
{
    public Dictionary<string,string> Save();
    public void Load(Dictionary<string,string> data);
}