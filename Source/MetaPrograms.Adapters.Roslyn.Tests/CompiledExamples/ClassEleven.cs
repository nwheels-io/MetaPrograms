using System.Threading.Tasks;

namespace MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples
{
    public class ClassEleven
    {
        public Task<ClassOne> GetOneAsync()
        {
            return Task.FromResult(new ClassOne());
        }
    }
}