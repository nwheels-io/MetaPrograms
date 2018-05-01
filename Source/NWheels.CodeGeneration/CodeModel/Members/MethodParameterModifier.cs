namespace NWheels.CodeGeneration.CodeModel.Members
{
    public enum MethodParameterModifier
    {
        None,
        Ref,
        Out,

        //currently not supported:
        //OutVar // out parameter with inline declaration of local variable ('declaration expression')
    }
}
