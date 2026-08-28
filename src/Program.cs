using MoonsecDeobfuscator.Deobfuscation;
using MoonsecDeobfuscator.Deobfuscation.Bytecode;

namespace MoonsecDeobfuscator;

public static class Program
{
    /*
        Devirtualize and dump bytecode to file:
            -dev -i <path to input> -o <path to output>

        Devirtualize and dump bytecode disassembly to file:
            -dis -i <path to input> -o <path to output>
    */

    static void Main(string[] args)
    {
        if (args.Length != 5 || args[1] != "-i" || args[3] != "-o")
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("Devirtualize and dump bytecode to file:\n\t-dev -i <input> -o <output>");
            Console.WriteLine("Devirtualize and dump bytecode disassembly to file:\n\t-dis -i <input> -o <output>");
            return;
        }

        var command = args[0];
        var input = args[2];
        var output = args[4];

        if (!File.Exists(input))
        {
            Console.WriteLine("Invalid input path!");
            return;
        }

        if (command == "-dev")
        {
            var deobfuscator = new Deobfuscator();
            var result = deobfuscator.Deobfuscate(File.ReadAllText(input));

            if (deobfuscator.Context.HadAntiTamper)
                Console.WriteLine("-- [[ ANTI-TAMPER BLOCK DETECTED ]]");

            using var stream = new FileStream(output, FileMode.Create, FileAccess.Write);
            using var serializer = new Serializer(stream);

            serializer.Serialize(result);
        }
        else if (command == "-dis")
        {
            var deobfuscator = new Deobfuscator();
            var result = deobfuscator.Deobfuscate(File.ReadAllText(input));

            var disassembly = new Disassembler(result).Disassemble();
            var outputText = deobfuscator.Context.HadAntiTamper
                ? $"-- [[ ANTI-TAMPER BLOCK ]]\n{disassembly}"
                : disassembly;

            File.WriteAllText(output, outputText);
        }
        else
        {
            Console.WriteLine("Invalid command!");
        }
    }
}
