using TheSingularityWorkshop.FSM_Serialization;

namespace TheSingularityWorkshop.Workshop.IO;

public interface IFileSystem
{
    bool Exists(string path);
    void Delete(string path);
    void CreateDirectory(string path);
    IBinaryStream OpenRead(string path);
    IBinaryStream OpenWrite(string path);
}
