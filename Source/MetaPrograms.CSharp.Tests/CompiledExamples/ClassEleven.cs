using System.Threading.Tasks;

namespace MetaPrograms.CSharp.Tests.CompiledExamples
{
    public class ClassEleven
    {
        public Task<ClassOne> GetOneAsync()
        {
            return Task.FromResult(new ClassOne());
        }
    }
}