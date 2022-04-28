using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NetcoreSaas.Application.Services.Images
{
    public interface IOpticalCharacterRecognitionService
    {
        Task<IEnumerable<string>> ReadLinesFromImage(FileStream stream);
    }
}
