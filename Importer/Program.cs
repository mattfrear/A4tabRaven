using Services;

namespace Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            new ImportService().Import();
        }
    }
}
